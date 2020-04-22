using EntityFramework.Extensions;
using Infrastructure.Utility;
using NewMK.Domian.DomainException;
using NewMK.DTO;
using NewMK.DTO.AdminUser;
using NewMK.DTO.ManageData;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DM
{
    public class AdminUserDM
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AdminUserDTO LoginUser(string name, string pwd, string SystemName)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                var sha512 = System.Security.Cryptography.SHA512.Create();
                byte[] pwa2 = sha512.ComputeHash(Encoding.Default.GetBytes(pwd));
                string pp = Encoding.UTF8.GetString(pwa2);
                AdminUserDTO dto = c.AdminUser.Where(w => w.UserCode == name && w.UserPwd == pp && w.SystemName.Contains(SystemName)).Select(
                    x => new AdminUserDTO
                    {
                        ID = x.ID,
                        UserName = x.UserName,
                        UserCode = x.UserCode

                    }
                    ).FirstOrDefault();

                return dto;
            }
        }


        public List<AdminRoleDTO> GetAdminRoleList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<AdminRoleDTO> dto = c.AdminRole.Select(
                    x => new AdminRoleDTO
                    {
                        ID = x.ID,
                        RoleName = x.RoleName
                    }
                ).ToList();

                return dto;
            }
        }

        public bool AddAdminRole(string name)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminRole pt = new AdminRole();
                pt.ID = Guid.NewGuid();
                pt.RoleName = name;

                c.AdminRole.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public AdminRoleDTO GetAdminRoleFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminRoleDTO dto = c.AdminRole.Where(w => w.ID == id).Select(
                    x => new AdminRoleDTO
                    {
                        ID = x.ID,
                        RoleName = x.RoleName
                    }
                ).FirstOrDefault();

                return dto;
            }
        }

        public bool UpAdminRole(AdminRoleDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminRole pt = c.AdminRole.FirstOrDefault(n => n.ID == dto.ID);
                pt.RoleName = dto.RoleName;

                c.SaveChanges();
                return true;
            }
        }


        public bool deAdminRole(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminRole pt = c.AdminRole.FirstOrDefault(n => n.ID == id);

                c.AdminRole.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        public List<AdminUserDTO> GetAdminUserList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<AdminUserDTO> dto = c.AdminUser.Select(
                    x => new AdminUserDTO
                    {
                        ID = x.ID,
                        RoleName = c.AdminRole.Where(w => w.ID == x.RoleID).FirstOrDefault().RoleName,
                        RoleID = x.RoleID,
                        UserCode = x.UserCode,
                        UserName = x.UserName,
                        UserPhone = x.UserPhone,
                        UserPwd = x.UserPwd,
                        SystemName = x.SystemName
                    }
                ).ToList();

                return dto;
            }
        }

        public bool AddAdminUser(AdminUserDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminUser au = c.AdminUser.Where(w => w.UserCode == dto.UserCode).FirstOrDefault();
                if (au != null)
                {
                    throw new DMException("该编号用户已经存在！");
                }
                var sha512 = System.Security.Cryptography.SHA512.Create();
                byte[] pwa2 = sha512.ComputeHash(Encoding.Default.GetBytes(dto.UserPwd));

                AdminUser pt = new AdminUser();
                pt.ID = Guid.NewGuid();
                pt.UserPwd = Encoding.UTF8.GetString(pwa2);
                pt.UserPhone = dto.UserPhone;
                pt.UserName = dto.UserName;
                pt.UserCode = dto.UserCode;
                pt.RoleID = dto.RoleID;
                pt.SystemName = dto.SystemName;

                c.AdminUser.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public AdminUserDTO GetAdminUserFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminUserDTO dto = c.AdminUser.Where(w => w.ID == id).Select(
                    x => new AdminUserDTO
                    {
                        ID = x.ID,
                        RoleName = c.AdminRole.Where(w => w.ID == x.RoleID).FirstOrDefault().RoleName,
                        RoleID = x.RoleID,
                        UserCode = x.UserCode,
                        UserName = x.UserName,
                        UserPhone = x.UserPhone,
                        UserPwd = x.UserPwd,
                        SystemName = x.SystemName
                    }
                ).FirstOrDefault();

                return dto;
            }
        }
        public AdminUserDTO GetAdminUserCode(string code)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminUserDTO dto = c.AdminUser.Where(w => w.UserCode == code).Select(
                    x => new AdminUserDTO
                    {
                        UserPhone = x.UserPhone,
                    }
                ).FirstOrDefault();

                return dto;
            }
        }

        public bool deAdminUser(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminUser pt = c.AdminUser.FirstOrDefault(n => n.ID == id);

                c.AdminUser.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        public bool UpAdminUser(AdminUserDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminUser au = c.AdminUser.Where(w => w.UserCode == dto.UserCode && w.ID != dto.ID).FirstOrDefault();
                if (au != null)
                {
                    throw new DMException("该编号用户已经存在！");
                }

                AdminUser pt = c.AdminUser.FirstOrDefault(n => n.ID == dto.ID);
                pt.RoleID = dto.RoleID;
                pt.UserCode = dto.UserCode;
                pt.UserName = dto.UserName;
                pt.UserPhone = dto.UserPhone;
                if (!string.IsNullOrEmpty(dto.UserPwd))
                {
                    var sha512 = System.Security.Cryptography.SHA512.Create();
                    byte[] pwa2 = sha512.ComputeHash(Encoding.Default.GetBytes(dto.UserPwd));

                    pt.UserPwd = Encoding.UTF8.GetString(pwa2);
                }

                pt.SystemName = dto.SystemName;

                c.SaveChanges();
                return true;
            }
        }

        public List<AdminMenuDTO> GetAdminMenuList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<AdminMenuDTO> dto = c.AdminMenu.OrderBy(x => x.Sort).ThenBy(x => x.MenuCode).Select(
                    x => new AdminMenuDTO
                    {
                        ID = x.ID,
                        MenuCode = x.MenuCode,
                        MenuExplain = x.MenuExplain,
                        MenuUrl = x.MenuUrl,
                        FID = x.FID,
                        IsShow = x.IsShow.Value,
                        SystemName = x.SystemName,
                        Type = (int)Enum.MenuType.Menu,
                        Sort = x.Sort.HasValue ? x.Sort.Value : 0
                    }
                ).ToList();

                List<AdminMenuActionDTO> actionDto = c.AdminMenuAction.Select(
                    x => new AdminMenuActionDTO
                    {
                        ID = x.ID,
                        Action = x.Action,
                        Comment = x.Comment,
                        MenuID = x.MenuID
                    }).ToList();

                if (actionDto != null && actionDto.Count > 0)
                {
                    foreach (var action in actionDto)
                    {
                        dto.Add(new AdminMenuDTO
                        {
                            ID = action.ID,
                            MenuCode = action.Action,
                            MenuExplain = action.Comment,
                            MenuUrl = "",
                            FID = action.MenuID,
                            Type = (int)Enum.MenuType.MenuAction,
                            Sort = 0
                        });
                    }
                }

                return dto;
            }
        }

        public bool AddAdminMenu(AdminMenuDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminMenu am = c.AdminMenu.Where(w => w.MenuCode == dto.MenuCode).FirstOrDefault();
                if (am != null)
                {
                    throw new DMException("该菜单编码已经存在！");
                }
                AdminMenu pt = new AdminMenu();
                pt.ID = Guid.NewGuid();
                pt.MenuCode = dto.MenuCode;
                pt.MenuExplain = dto.MenuExplain;
                pt.MenuUrl = dto.MenuUrl;
                pt.FID = dto.FID;
                pt.IsShow = dto.IsShow;
                pt.SystemName = dto.SystemName;
                pt.Sort = dto.Sort;

                c.AdminMenu.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public AdminMenuDTO GetAdminMenuFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminMenuDTO dto = c.AdminMenu.Where(w => w.ID == id).Select(
                    x => new AdminMenuDTO
                    {
                        ID = x.ID,
                        MenuCode = x.MenuCode,
                        MenuExplain = x.MenuExplain,
                        MenuUrl = x.MenuUrl,
                        SystemName = x.SystemName,
                        Sort = x.Sort.Value,
                        IsShow = x.IsShow.Value,
                        FID = x.FID
                    }
                ).FirstOrDefault();

                return dto;
            }
        }

        public bool UpAdminMenu(AdminMenuDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminMenu am = c.AdminMenu.Where(w => w.MenuCode == dto.MenuCode && w.ID != dto.ID).FirstOrDefault();
                if (am != null)
                {
                    throw new DMException("该菜单编码已经存在！");
                }

                AdminMenu pt = c.AdminMenu.FirstOrDefault(n => n.ID == dto.ID);
                pt.MenuCode = dto.MenuCode;
                pt.MenuExplain = dto.MenuExplain;
                pt.MenuUrl = dto.MenuUrl;
                pt.FID = dto.FID;
                pt.IsShow = dto.IsShow;
                pt.SystemName = dto.SystemName;
                pt.Sort = dto.Sort;

                c.SaveChanges();
                return true;
            }
        }

        public bool deAdminMenu(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminMenu pt = c.AdminMenu.FirstOrDefault(n => n.ID == id);
                //判断是否存在子菜单，若存在子菜单就不能删除
                AdminMenu childTop = c.AdminMenu.FirstOrDefault(n => n.FID == id);
                if (childTop != null)
                {
                    throw new DMException("存在子菜单，无法删除！");
                }

                List<RoleMenu> roleMenus = c.RoleMenu.Where(n => n.MenuID == id).ToList();
                if (roleMenus != null && roleMenus.Count > 0)
                {
                    foreach (var roleMenu in roleMenus)
                    {
                        c.RoleMenu.Remove(roleMenu);
                    }
                }

                List<AdminMenuAction> actions = c.AdminMenuAction.Where(n => n.MenuID == id).ToList();
                if (actions != null && actions.Count > 0)
                {
                    foreach (var action in actions)
                    {
                        c.AdminMenuAction.Remove(action);
                    }
                }

                c.AdminMenu.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        public List<AdminMenuDTO> RoleMenuList(Guid rid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<AdminMenuDTO> dto = c.AdminMenu.Where(w => w.RoleMenu.Any(l => l.RoleID == rid)).Select(
                    x => new AdminMenuDTO
                    {
                        ID = x.ID,
                        MenuCode = x.MenuCode,
                        MenuExplain = x.MenuExplain,
                        MenuUrl = x.MenuUrl,
                        Type = (int)Enum.MenuType.Menu
                    }
                ).ToList();

                List<AdminMenuActionDTO> actionDto = c.AdminMenuAction.Where(w => w.RoleMenuAction.Any(l => l.RoleID == rid)).Select(
                    x => new AdminMenuActionDTO
                    {
                        ID = x.ID,
                        Action = x.Action,
                        MenuID = x.MenuID,
                        Comment = x.Comment
                    }
                ).ToList();
                if (actionDto != null && actionDto.Count > 0)
                {
                    foreach (var action in actionDto)
                    {
                        dto.Add(new AdminMenuDTO()
                        {
                            ID = action.ID,
                            MenuCode = action.Action,
                            MenuExplain = action.Comment,
                            MenuUrl = "",
                            Type = (int)Enum.MenuType.MenuAction
                        });

                    }
                }

                return dto;
            }
        }

        public bool UpRoleMenu(RoleMenuDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (var ctxTransaction = c.Database.BeginTransaction())
                {
                    try
                    {
                        c.RoleMenu.Where(f => f.RoleID == dto.rid).Delete();
                        c.RoleMenuAction.Where(f => f.RoleID == dto.rid).Delete();

                        if (dto.mlist != null)
                        {
                            foreach (string item in dto.mlist)
                            {
                                log.Debug($"权限信息：{item.ToJSON()}");
                                string[] ss = item.Split('|');
                                byte type = byte.Parse(ss[0]);
                                Guid id = Guid.Parse(ss[1]);

                                if (type == (int)Enum.MenuType.Menu)//菜单
                                {
                                    c.RoleMenu.Add(new RoleMenu
                                    {
                                        ID = Guid.NewGuid(),
                                        MenuID = id,
                                        RoleID = dto.rid
                                    });
                                }
                                else//菜单行为
                                {
                                    var action = c.AdminMenuAction.Where(f => f.ID == id).FirstOrDefault();
                                    c.RoleMenuAction.Add(new RoleMenuAction
                                    {
                                        ID = Guid.NewGuid(),
                                        MenuID = action.MenuID,
                                        ActionID = action.ID,
                                        RoleID = dto.rid
                                    });
                                }
                            }
                        }

                        c.SaveChanges();
                        ctxTransaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ctxTransaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public List<AdminMenuDTO> GetUserRoleMenu(Guid userID, string systemName)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminUser au = c.AdminUser.Where(w => w.ID == userID).FirstOrDefault();
                if (au == null)
                {
                    throw new DMException("用户不存在！");
                }
                List<AdminMenuDTO> dto = c.AdminMenu.Where(w => w.RoleMenu.Any(l => l.RoleID == au.RoleID) && w.SystemName == systemName).OrderBy(x => x.Sort).ThenBy(x => x.MenuCode).ThenBy(x => x.MenuCode).Select(
                    x => new AdminMenuDTO
                    {
                        ID = x.ID,
                        MenuCode = x.MenuCode,
                        MenuExplain = x.MenuExplain,
                        FID = x.FID,
                        MenuUrl = x.MenuUrl,
                        IsShow = x.IsShow.Value
                    }
                ).ToList();
                var query = (
                    from a in c.RoleMenuAction.Where(w => w.RoleID == au.RoleID)
                    join b in c.AdminMenuAction on a.ActionID equals b.ID// into b_join
                    //from b in b_join.DefaultIfEmpty()
                    select new AdminMenuActionDTO()
                    {
                        ID = b.ID,
                        Action = b.Action,
                        MenuID = b.MenuID
                    });
                var actionDto = query.ToList();
                if (actionDto != null && actionDto.Count > 0)
                {
                    foreach (var action in actionDto)
                    {
                        var menu = dto.Where(w => w.ID == action.MenuID).FirstOrDefault();
                        if (menu == null)
                        {
                            continue;
                        }
                        if (menu.Action == null)
                        {
                            menu.Action = new List<AdminMenuActionDTO>();
                        }
                        menu.Action.Add(action);
                    }
                }

                return dto;
            }
        }


        public List<AdminMenuActionDTO> GetAdminMenuAction(Guid menuId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<AdminMenuActionDTO> dto = c.AdminMenuAction.Where(w => w.MenuID == menuId).Select(
                    x => new AdminMenuActionDTO
                    {
                        ID = x.ID,
                        Action = x.Action,
                        MenuID = x.MenuID,
                        Comment = x.Comment
                    }
                ).ToList();

                return dto;
            }
        }


        public bool UpAdminMenuAction(AdminMenuActionDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminMenuAction am = c.AdminMenuAction.Where(w => w.MenuID == dto.MenuID && w.Action.Equals(dto.Action) && w.ID != dto.ID).FirstOrDefault();
                if (am != null)
                {
                    throw new DMException("菜单的行为已经存在！");
                }
                AdminMenuAction pt = c.AdminMenuAction.FirstOrDefault(n => n.ID == dto.ID);
                pt.Action = dto.Action;
                pt.MenuID = dto.MenuID;
                pt.Comment = dto.Comment;

                c.SaveChanges();
                return true;
            }
        }

        public bool AddAdminMenuAction(AdminMenuActionDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                AdminMenuAction am = c.AdminMenuAction.Where(w => w.MenuID == dto.MenuID && w.Action.Equals(dto.Action)).FirstOrDefault();
                if (am != null)
                {
                    throw new DMException("菜单的行为已经存在！");
                }
                AdminMenuAction pt = new AdminMenuAction();
                pt.ID = Guid.NewGuid();
                pt.MenuID = dto.MenuID;
                pt.Action = dto.Action;
                pt.Comment = dto.Comment;

                c.AdminMenuAction.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public bool deAdminMenuAction(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<RoleMenuAction> roleMenuActions = c.RoleMenuAction.Where(x => x.ActionID == id).ToList();
                if (roleMenuActions != null && roleMenuActions.Count > 0)
                {
                    foreach (var roleMenu in roleMenuActions)
                    {
                        c.RoleMenuAction.Remove(roleMenu);
                    }
                }

                AdminMenuAction pt = c.AdminMenuAction.FirstOrDefault(n => n.ID == id);
                if (pt != null)
                {
                    c.AdminMenuAction.Remove(pt);
                }
                c.SaveChanges();
                return true;
            }
        }


        public List<FreightRulesDTO> GetFreightRules(FreightRulesDTO pdl, out int pagecount)
        {
            System.Linq.Expressions.Expression<Func<FreightRules, bool>> expr = n => true;

            if (pdl.ProvinceID != null)
            {
                expr = expr.And2(n => n.ProvinceID == pdl.ProvinceID);
            }

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                pagecount = c.FreightRules.Where(expr).ToList().Count;
                List<FreightRulesDTO> dto = c.FreightRules.Where(expr).OrderBy(px => px.ProvinceName)
                    .Skip((pdl.pageIndex - 1) * pdl.pageSize).Take(pdl.pageSize).Select(
                    x => new FreightRulesDTO
                    {
                        ID = x.ID,
                        Price1 = x.Price1,
                        Price2 = x.Price2,
                        Price3 = x.Price3,
                        Price4 = x.Price4,
                        ProvinceID = x.ProvinceID,
                        ProvinceName = x.ProvinceName

                    }
                ).ToList();

                return dto;
            }
        }

        public bool AddYfgz(FreightRulesDTO pdl)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                FreightRules pt = new FreightRules();
                pt.ID = Guid.NewGuid();
                pt.Price1 = pdl.Price1;
                pt.Price2 = pdl.Price2;
                pt.Price3 = pdl.Price3;
                pt.Price4 = pdl.Price4;
                pt.ProvinceID = pdl.ProvinceID;
                pt.ProvinceName = c.Areas.Where(w => w.ID == pdl.ProvinceID).FirstOrDefault().Province;


                c.FreightRules.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public bool deYfgz(Guid? id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                FreightRules pt = c.FreightRules.FirstOrDefault(n => n.ID == id);

                c.FreightRules.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        public List<AdminUserOperationLogDTO> GetAdminUserOperationLog(Request_AdminUserOperationLogDTO dto, out int count)
        {
            List<AdminUserOperationLogDTO> dtoli = new List<AdminUserOperationLogDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {


                Expression<Func<AdminUserOperationLog, bool>> expr = AutoAssemble.Splice<AdminUserOperationLog, Request_AdminUserOperationLogDTO>(dto);
                if (dto.STime != null)
                {
                    expr = expr.And2(w => w.AddTime >= dto.STime);
                }
                if (dto.ETime != null)
                {
                    expr = expr.And2(w => w.AddTime <= dto.ETime);
                }

                count = c.AdminUserOperationLog.Where(expr).Count();
                List<AdminUserOperationLog> li = c.AdminUserOperationLog.Where(expr).OrderByDescending(px => px.AddTime).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();

                dtoli = GetMapperDTO.GetDTOList<AdminUserOperationLog, AdminUserOperationLogDTO>(li);
            }

            return dtoli;
        }



    }
}