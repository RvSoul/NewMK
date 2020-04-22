using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.AdminUser;
using NewMK.DTO.ManageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DomainEnum = NewMK.Domian.Enum;

namespace Manage.NewMK.WebApi.Controllers
{
    [Authorize]
    public class AdminUserController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        AdminUserDM dm = new AdminUserDM();


        /// <summary>
        /// 根据用户编号获取用户电话
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GetUser")]
        public ResultEntity<AdminUserDTO> GetAdminUserCode(string code)
        {
            return new ResultEntityUtil<AdminUserDTO>().Success(dm.GetAdminUserCode(code));
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUser")]
        public ResultEntity<AdminUserDTO> GetUser()
        {
            return new ResultEntityUtil<AdminUserDTO>().Success(dm.GetAdminUserFirst(CurrentUserId));

        }

        #region 权限管理-角色管理
        [HttpGet]
        [Route("api/GetAdminRoleList")]
        public ResultEntity<List<AdminRoleDTO>> GetAdminRoleList()
        {
            return new ResultEntityUtil<List<AdminRoleDTO>>().Success(dm.GetAdminRoleList());

        }

        [HttpGet]
        [Route("api/AddAdminRole")]
        public ResultEntity<bool> AddAdminRole(string name)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddAdminRole(name));

        }


        [HttpGet]
        [Route("api/GetAdminRoleFirst")]
        public ResultEntity<AdminRoleDTO> GetAdminRoleFirst(Guid id)
        {
            return new ResultEntityUtil<AdminRoleDTO>().Success(dm.GetAdminRoleFirst(id));

        }

        [HttpGet]
        [Route("api/UpAdminRole")]
        public ResultEntity<bool> UpAdminRole([FromUri]AdminRoleDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpAdminRole(dto));

        }

        [HttpGet]
        [Route("api/deAdminRole")]
        public ResultEntity<bool> deAdminRole(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deAdminRole(id));

        }


        #endregion

        #region 权限管理-用户管理

        [HttpGet]
        [Route("api/GetAdminUserList")]
        public ResultEntity<List<AdminUserDTO>> GetAdminUserList()
        {
            return new ResultEntityUtil<List<AdminUserDTO>>().Success(dm.GetAdminUserList());

        }

        [HttpGet]
        [Route("api/AddAdminUser")]
        public ResultEntity<bool> AddAdminUser([FromUri]AdminUserDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddAdminUser(dto));

        }

        [HttpGet]
        [Route("api/GetAdminUserFirst")]
        public ResultEntity<AdminUserDTO> GetAdminUserFirst(Guid id)
        {
            return new ResultEntityUtil<AdminUserDTO>().Success(dm.GetAdminUserFirst(id));

        }

        [HttpGet]
        [Route("api/UpAdminUser")]
        public ResultEntity<bool> UpAdminUser([FromUri]AdminUserDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpAdminUser(dto));

        }

        [HttpGet]
        [Route("api/deAdminUser")]
        public ResultEntity<bool> deAdminUser(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deAdminUser(id));

        }
        #endregion

        #region 权限管理-菜单管理
        [HttpGet]
        [Route("api/GetAdminMenuList")]
        public ResultEntity<List<AdminMenuDTO>> GetAdminMenuList()
        {
            return new ResultEntityUtil<List<AdminMenuDTO>>().Success(dm.GetAdminMenuList());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddAdminMenu")]
        public ResultEntity<bool> AddAdminMenu([FromUri]AdminMenuDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddAdminMenu(dto));

        }

        [HttpGet]
        [Route("api/GetAdminMenuFirst")]
        public ResultEntity<AdminMenuDTO> GetAdminMenuFirst(Guid id)
        {
            return new ResultEntityUtil<AdminMenuDTO>().Success(dm.GetAdminMenuFirst(id));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpAdminMenu")]
        public ResultEntity<bool> UpAdminMenu([FromUri]AdminMenuDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpAdminMenu(dto));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deAdminMenu")]
        public ResultEntity<bool> deAdminMenu(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deAdminMenu(id));


        }
        #endregion


        #region 权限管理-行为管理

        [HttpGet]
        [Route("api/GetAdminMenuAction")]

        public ResultEntity<List<AdminMenuActionDTO>> GetAdminMenuAction(Guid id)
        {
            return new ResultEntityUtil<List<AdminMenuActionDTO>>().Success(dm.GetAdminMenuAction(id));

        }


        [HttpGet]
        [Route("api/AddAdminMenuAction")]

        public ResultEntity<bool> AddAdminMenuAction([FromUri]AdminMenuActionDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddAdminMenuAction(dto));

        }

        [HttpGet]
        [Route("api/UpAdminMenuAction")]

        public ResultEntity<bool> UpAdminMenuAction([FromUri]AdminMenuActionDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpAdminMenuAction(dto));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deAdminMenuAction")]
        public ResultEntity<bool> deAdminMenuAction(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deAdminMenuAction(id));


        }


        #endregion

        #region 分配权限

        [HttpGet]
        [Route("api/RoleMenuList")]
        public ResultEntity<List<AdminMenuDTO>> RoleMenuList(Guid rid)
        {
            return new ResultEntityUtil<List<AdminMenuDTO>>().Success(dm.RoleMenuList(rid));

        }

        [HttpPost]
        [Route("api/UpRoleMenu")]
        public ResultEntity<bool> UpRoleMenu([FromBody]RoleMenuDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpRoleMenu(dto));

        }
        [HttpGet]
        [Route("api/GetUserRoleMenu")]
        public ResultEntity<List<AdminMenuDTO>> GetUserRoleMenu(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
            {
                systemName = (int)DomainEnum.SystemName.后台系统 + "";
            }
            return new ResultEntityUtil<List<AdminMenuDTO>>().Success(dm.GetUserRoleMenu(CurrentUserId, systemName));
        }

        #endregion

        #region 运费规则
        /// <summary>
        /// 获取运费规则
        /// </summary>
        /// <param name="pdl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetFreightRules")]
        public ResultEntity<List<FreightRulesDTO>> GetFreightRules([FromUri] FreightRulesDTO pdl)
        {
            int pagecount = 0;
            return new ResultEntityUtil<List<FreightRulesDTO>>().Success(dm.GetFreightRules(pdl, out pagecount), pagecount);

        }

        /// <summary>
        /// 添加运费规则
        /// </summary>
        /// <param name="pdl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddYfgz")]
        public ResultEntity<bool> AddYfgz([FromUri] FreightRulesDTO pdl)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddYfgz(pdl));

        }

        /// <summary>
        /// 删除运费规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deYfgz")]
        public ResultEntity<bool> deYfgz(Guid? id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deYfgz(id));

        }
        #endregion


        /// <summary>
        /// 获取省市县地址
        /// </summary>
        /// <param name="id">父级ID</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAreaList")]
        public ResultEntity<List<AreasDTO>> GetAreaList(int? id, int level)
        {
            ManageDataDM dm = new ManageDataDM();
            return new ResultEntityUtil<List<AreasDTO>>().Success(dm.GetAreaList(id, level));

        }
        [HttpGet]
        [Route("api/GetAdminUserOperationLog")]
        public ResultEntity<List<AdminUserOperationLogDTO>> GetAdminUserOperationLog([FromUri]Request_AdminUserOperationLogDTO dto)
        {
            int count = 0;
            return new ResultEntityUtil<List<AdminUserOperationLogDTO>>().Success(dm.GetAdminUserOperationLog(dto, out count), count);

        }



        
    }
}
