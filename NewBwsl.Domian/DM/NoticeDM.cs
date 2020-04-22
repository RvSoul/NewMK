using NewMK.DTO;
using NewMK.DTO.Notice;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DM
{
    public class NoticeDM
    {
        #region 产品类型操作
        public List<NoticeTypeDTO> GetNoticeType()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<NoticeTypeDTO> userDto = c.NoticeType.OrderByDescending(p => p.PX).Select(
                    x => new NoticeTypeDTO
                    {
                        ID = x.ID,
                        Name = x.Name,
                        PX = x.PX
                    }
                ).ToList();

                return userDto;
            }
        }


        public bool AddNoticeType(string name, int? px)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                NoticeType pt = new NoticeType();
                pt.ID = Guid.NewGuid();
                pt.Name = name;
                pt.PX = px;

                c.NoticeType.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public NoticeTypeDTO GetNoticeTypeFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                NoticeTypeDTO userDto = c.NoticeType.Where(n => n.ID == id).Select(
                    x => new NoticeTypeDTO
                    {
                        ID = x.ID,
                        Name = x.Name,
                        PX = x.PX
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpNoticeType(Guid id, string name, int? px)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                NoticeType pt = c.NoticeType.FirstOrDefault(n => n.ID == id);

                pt.Name = name;
                pt.PX = px;

                c.SaveChanges();
                return true;
            }
        }



        public bool deNoticeType(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                NoticeType pt = c.NoticeType.FirstOrDefault(n => n.ID == id);

                c.NoticeType.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #region 产品操作
        public List<NoticeDTO> GetNoticeList(ModelDTO rDto, out int pagcount)
        {
            Expression<Func<Notice, bool>> expr = n => true;
            expr = expr.And2(w => w.State == true);
            expr = expr.And2(w => w.EndTime >= DateTime.Now && w.StartTime <= DateTime.Now);

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                pagcount = c.Notice.Where(expr).ToList().Count;
                List<NoticeDTO> dto = c.Notice.Where(expr).OrderByDescending(o => o.CreateTime).Skip((rDto.PageIndex - 1) * rDto.PageSize).Take(rDto.PageSize).Select(
                    x => new NoticeDTO
                    {
                        ID = x.ID,
                        NoticeTypeID = x.NoticeTypeID,
                        AdminUserID = x.AdminUserID,
                        CreateTime = x.CreateTime,
                        Descn = x.Descn,
                        EndTime = x.EndTime,
                        ReadQuantity = x.ReadQuantity,
                        StartTime = x.StartTime,
                        Title = x.Title,
                        State = x.State,
                        NoticeTypeName = x.NoticeType.Name

                    }
                ).ToList();


                return dto;
            }
        }

        public List<NoticeDTO> GetNoticeList(Request_NoticeDTO rDto, out int pagcount)
        {
            //System.Linq.Expressions.Expression<Func<Notice, bool>> expr = n => true;
            Expression<Func<Notice, bool>> expr = AutoAssemble.Splice<Notice, Request_NoticeDTO>(rDto);

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                pagcount = c.Notice.Where(expr).ToList().Count;
                List<NoticeDTO> dto = c.Notice.Where(expr).OrderByDescending(o => o.CreateTime).Skip((rDto.PageIndex - 1) * rDto.PageSize).Take(rDto.PageSize).Select(
                    x => new NoticeDTO
                    {
                        ID = x.ID,
                        NoticeTypeID = x.NoticeTypeID,
                        AdminUserID = x.AdminUserID,
                        CreateTime = x.CreateTime,
                        Descn = x.Descn,
                        EndTime = x.EndTime,
                        ReadQuantity = x.ReadQuantity,
                        StartTime = x.StartTime,
                        Title = x.Title,
                        State = x.State,
                        AdminUserName = x.AdminUser.UserName,
                        NoticeTypeName = x.NoticeType.Name

                    }
                ).ToList();


                return dto;
            }
        }

        public NoticeDTO GetNotice(Guid? ID)
        {

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                NoticeDTO dto = c.Notice.Where(w => w.ID == ID).Select(
                    x => new NoticeDTO
                    {
                        ID = x.ID,
                        NoticeTypeID = x.NoticeTypeID,
                        AdminUserID = x.AdminUserID,
                        CreateTime = x.CreateTime,
                        Descn = x.Descn,
                        EndTime = x.EndTime,
                        ReadQuantity = x.ReadQuantity,
                        StartTime = x.StartTime,
                        Title = x.Title,
                        State = x.State,
                        AdminUserName = x.AdminUser.UserName,
                        NoticeTypeName = x.NoticeType.Name

                    }
                ).FirstOrDefault();

                return dto;
            }
        }

        public bool AddNotice(NoticeModel dt, Guid userId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Notice pt = new Notice();
                pt.ID = Guid.NewGuid();
                pt.NoticeTypeID = dt.NoticeTypeID;
                pt.AdminUserID = userId;
                pt.CreateTime = DateTime.Now;
                pt.Descn = dt.Descn;
                pt.EndTime = dt.EndTime;
                pt.ReadQuantity = dt.ReadQuantity;
                pt.StartTime = dt.StartTime;
                pt.Title = dt.Title;
                pt.State = dt.State;


                c.Notice.Add(pt);
                c.SaveChanges();
                return true;
            }
        }
        public bool UpNotice(NoticeModel dt, Guid userId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Notice pt = c.Notice.Where(w => w.ID == dt.ID).FirstOrDefault();
                pt.NoticeTypeID = dt.NoticeTypeID;
                pt.AdminUserID = userId;
                //pt.CreateTime = dt.CreateTime;
                pt.Descn = dt.Descn;
                pt.EndTime = dt.EndTime;
                pt.ReadQuantity = dt.ReadQuantity;
                pt.StartTime = dt.StartTime;
                pt.Title = dt.Title;
                pt.State = dt.State;

                c.SaveChanges();
                return true;
            }

        }
        public bool deNotice(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Notice pt = c.Notice.FirstOrDefault(n => n.ID == id);

                c.Notice.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        #endregion
    }
}
