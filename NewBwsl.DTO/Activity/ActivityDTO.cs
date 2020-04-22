using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewMK.DTO.Activity
{

    public class ActivityTT
    {
        
        public System.Guid ID { get; set; }

        [Required(ErrorMessage = "活动名字不能为空！")]
        public string ActivityName { get; set; }
               
        public System.DateTime StartTime { get; set; }
        
        public System.DateTime EndTime { get; set; }
        
        public int ActivityObjectForm { get; set; }
        
        public int ActivityForm { get; set; }
        
        public string ActivityExplain { get; set; }
        public Nullable<decimal> BasicsMoney { get; set; }
        public Nullable<decimal> DiscountMoney { get; set; }
        
        public bool IfAll { get; set; }
        
        public bool IfEnable { get; set; }
        public Nullable<int> BuyNumber { get; set; }
        public Nullable<bool> IfDoubleGive { get; set; }
    }
    public class ActivityDTO
    {
        public static List<ActivityDTO> GetActivityDTOList(List<Model.CM.Activity> data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.CM.Activity, ActivityDTO>();
                cfg.CreateMap<Model.CM.ActivityUserType, ActivityUserTypeDTO>();
                cfg.CreateMap<Model.CM.ActivityGifts, ActivityGiftsDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<Model.CM.Activity>, List<ActivityDTO>>(data);
        }


        public System.Guid ID { get; set; }
        public string ActivityName { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> ActivityObjectForm { get; set; }
        public Nullable<int> ActivityForm { get; set; }
        public string ActivityExplain { get; set; }
        public Nullable<decimal> BasicsMoney { get; set; }
        public Nullable<decimal> DiscountMoney { get; set; }
        public Nullable<bool> IfAll { get; set; }
        public Nullable<bool> IfEnable { get; set; }
        public Nullable<int> BuyNumber { get; set; }
        public Nullable<bool> IfDoubleGive { get; set; }

        public List<ActivityUserTypeDTO> ActivityUserType { get; set; }
        public List<ActivityGiftsDTO> ActivityGifts { get; set; }
    }
}
