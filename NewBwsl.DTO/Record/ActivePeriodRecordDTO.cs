using AutoMapper;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Record
{
    public class ActivePeriodRecordDTO
    {
        public static List<ActivePeriodRecordDTO> GetDTO(List<ActivePeriodRecord> data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ActivePeriodRecord, ActivePeriodRecordDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<ActivePeriodRecord>, List<ActivePeriodRecordDTO>>(data);
        }

        public System.Guid ID { get; set; }
        public System.Guid UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int RecordType { get; set; }
        public Nullable<int> StageOne { get; set; }
        public Nullable<int> StageTwo { get; set; }
        public Nullable<int> StageThree { get; set; }
        public decimal PVSurplus { get; set; }
        public System.DateTime AddTime { get; set; }
        public string Remarks { get; set; }
    }

    public class Request_ActivePeriodRecordDTO:ModelDTO
    {
        [SelectField("and", "=", "string")]
        public string UserCode { get; set; }

        [SelectField("and", "=", "string")]
        public string UserName { get; set; }

        [SelectField("and", "=", "int")]
        public int? RecordType { get; set; }
    }
}
