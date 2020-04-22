
using NewMK.DTO.ManageData;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DM
{
    public class ManageDataDM
    {
        public List<AreasDTO> GetAreaList(int? id, int level)
        {
            System.Linq.Expressions.Expression<Func<Areas, bool>> expr = n => true;


            expr = expr.And2(n => n.PerantId == id);
            expr = expr.And2(n => n.PerantId == id);

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<AreasDTO> userDto = c.Areas.Where(expr).OrderBy(p => p.ID).Select(
                    x => new AreasDTO
                    {
                        ID = x.ID,
                        Province = x.Province,
                        City = x.City,
                        County = x.County,
                        Town = x.Town,
                        AreaFullName = x.AreaFullName,
                        Level = x.Level,
                        PerantId = x.PerantId
                    }
                ).ToList();

                return userDto;
            }
        }
    }
}
