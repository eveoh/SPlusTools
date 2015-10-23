using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UvA.SPlusTools.Data.Entities
{
    [DataContract]
    partial class Activity
    {
        [DataMember]
        public bool IsScheduled { get { return Object.SchedulingStatus == 0; } }
        [DataMember]
        public DayOfWeek[] SuggestedDaysInWeek
        {
            get { return ((object[])Object.SuggestedDaysInWeek).Select(r => College.GetDay((int)r)).ToArray(); }
            set { Object.SuggestedDaysInWeek = value.Select(r => r.ToSplusDay()).Cast<object>().ToArray(); }
        }
        [DataMember]
        public TimeSpan? SuggestedStartTime
        {
            get { return SuggestedPeriodInDay == -1 ? (TimeSpan?)null : College.PeriodToTime(SuggestedPeriodInDay); }
            set { SuggestedPeriodInDay = value == null ? -1 : College.TimeToPeriod(value.Value); }
        }

        ResourceRequirement<Location> _LocationRequirement;
        [DataMember]
        public ResourceRequirement<Location> LocationRequirement { get { return _LocationRequirement = _LocationRequirement ?? new ResourceRequirement<Location>(this); } }

        ResourceRequirement<StaffMember> _StaffRequirement;
        [DataMember]
        public ResourceRequirement<StaffMember> StaffRequirement { get { return _StaffRequirement = _StaffRequirement ?? new ResourceRequirement<StaffMember>(this); } }

        ResourceRequirement<StudentSet> _StudentSetRequirement;
        [DataMember]
        public ResourceRequirement<StudentSet> StudentSetRequirement { get { return _StudentSetRequirement = _StudentSetRequirement ?? new ResourceRequirement<StudentSet>(this); } }

        [DataMember]
        public TimeSpan DurationTime { get { return TimeSpan.FromMinutes(Duration * College.PeriodLength); } set { Duration = (int)Math.Ceiling(value.TotalMinutes / College.PeriodLength); } }

        public bool ScheduleMany()
        {
            Object.ScheduleMany();
            return IsScheduled;
        }

        public bool Schedule(PeriodInWeekPattern pattern)
        {
            Object.Schedule(pattern.Object);
            return IsScheduled;
        }

        public DateTime StartDate
        {
            get { throw new NotImplementedException(); }
            set
            {
                SuggestedDaysInWeek = new DayOfWeek[] { value.DayOfWeek };
                WeekPattern = new WeekInYearPattern(College) { Weeks = new int[] { College.GetWeek(value) } };
            }
        }
    }
}
