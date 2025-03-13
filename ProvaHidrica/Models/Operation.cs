using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaHidrica.Models
{
    public class Operation(
        long operationId,
        string vp,
        string cis,
        string _operator,
        string recipe,
        string startTime,
        string endTime,
        string duration,
        bool infPoint1,
        bool infPoint2,
        bool infPoint3,
        bool infPoint4,
        bool infPoint5,
        bool infPoint6,
        bool infPoint7,
        bool infPoint8,
        bool infPoint9,
        bool infPoint10,
        bool infPoint11,
        bool infPoint12,
        bool infPoint13,
        bool infPoint14,
        bool infPoint15,
        bool infPoint16,
        bool infPoint17,
        bool infPoint18,
        bool infPoint19,
        bool infPoint20,
        bool infPoint21,
        bool infPoint22,
        bool infPoint23,
        bool infPoint24,
        bool infPoint25,
        bool infPoint26,
        bool infPoint27,
        bool infPoint28,
        bool infPoint29,
        bool infPoint30,
        bool infPoint31,
        bool infPoint32,
        bool infPoint33,
        DateTime createdAt
    )
    {
        public long OperationId { get; set; } = operationId;
        public string Vp { get; set; } = vp;
        public string Cis { get; set; } = cis;
        public string Operator { get; set; } = _operator;
        public string Recipe { get; set; } = recipe;
        public string StartTime { get; set; } = startTime;
        public string Duration { get; set; } = duration;
        public string EndTime { get; set; } = endTime;
        public bool InfPoint1 { get; set; } = infPoint1;
        public bool InfPoint2 { get; set; } = infPoint2;
        public bool InfPoint3 { get; set; } = infPoint3;
        public bool InfPoint4 { get; set; } = infPoint4;
        public bool InfPoint5 { get; set; } = infPoint5;
        public bool InfPoint6 { get; set; } = infPoint6;
        public bool InfPoint7 { get; set; } = infPoint7;
        public bool InfPoint8 { get; set; } = infPoint8;
        public bool InfPoint9 { get; set; } = infPoint9;
        public bool InfPoint10 { get; set; } = infPoint10;
        public bool InfPoint11 { get; set; } = infPoint11;
        public bool InfPoint12 { get; set; } = infPoint12;
        public bool InfPoint13 { get; set; } = infPoint13;
        public bool InfPoint14 { get; set; } = infPoint14;
        public bool InfPoint15 { get; set; } = infPoint15;
        public bool InfPoint16 { get; set; } = infPoint16;
        public bool InfPoint17 { get; set; } = infPoint17;
        public bool InfPoint18 { get; set; } = infPoint18;
        public bool InfPoint19 { get; set; } = infPoint19;
        public bool InfPoint20 { get; set; } = infPoint20;
        public bool InfPoint21 { get; set; } = infPoint21;
        public bool InfPoint22 { get; set; } = infPoint22;
        public bool InfPoint23 { get; set; } = infPoint23;
        public bool InfPoint24 { get; set; } = infPoint24;
        public bool InfPoint25 { get; set; } = infPoint25;
        public bool InfPoint26 { get; set; } = infPoint26;
        public bool InfPoint27 { get; set; } = infPoint27;
        public bool InfPoint28 { get; set; } = infPoint28;
        public bool InfPoint29 { get; set; } = infPoint29;
        public bool InfPoint30 { get; set; } = infPoint30;
        public bool InfPoint31 { get; set; } = infPoint31;
        public bool InfPoint32 { get; set; } = infPoint32;
        public bool InfPoint33 { get; set; } = infPoint33;
        public DateTime CreatedAt { get; set; } = createdAt;
    }
}
