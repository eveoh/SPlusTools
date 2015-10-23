using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace UvA.SPlusTools.Data.Entities
{
    public partial class College : ITimeObject
    {
        internal dynamic Object { get; private set; }
        internal dynamic Application { get; private set; }
        public int PeriodLength { get; private set; }

        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        public bool RefreshFinished { get; set; }


        Dictionary<string, SPlusObject> Objects;

        public College(string progID)
        {
            // Create the top level Syllabus object
            Application = ((dynamic)Activator.CreateInstance(Type.GetTypeFromProgID(progID + ".application")));
            // The activeCollege contains all activities and such
            Object = Application.ActiveCollege;
            Objects = new Dictionary<string, SPlusObject>();
            StartTime = ((DateTime)Object.StartTime).TimeOfDay;
            EndTime = ((DateTime)Object.EndTime).TimeOfDay;
            if (EndTime.TotalMinutes == 0)
                EndTime = TimeSpan.FromDays(1);
            PeriodLength = (int)EndTime.Subtract(StartTime).TotalMinutes / PeriodsPerDay;

            // Attempt to connect an eventhandler to the afterRefresh event. 
            //Application.SDB.AfterRefresh += new Action<Boolean, Boolean>(_Application_AfterRefresh);
            
        }

        public void _Application_AfterRefresh(Boolean success, Boolean changes)
        {
            this.RefreshFinished = true;
        }

        internal T GetObject<T>(dynamic obj) where T : SPlusObject
        {
            if (obj == null)
                return null;
            string id = obj.ObjectId;
            if (!Objects.ContainsKey(id))
                Objects.Add(obj.ObjectId, (T)Activator.CreateInstance(typeof(T), new object[] { this, obj }));
            return (T)Objects[id];
        }

        public Activity CreateActivity(DateTime date, double startTime, double endTime, Location loc)
        {
            Activity act = new Activity(this);

            return act;
        }

        public void DeleteActivity(Activity act)
        {
            Object.DeleteActivity(act.Object);
        }

        public void WriteBack()
        {
            Application.SDB.Writeback();
        }

        public void Refresh()
        {
            this.RefreshFinished = false;
            Application.SDB.Refresh();
        }

        public delegate void AfterRefreshHandler(Boolean success, Boolean changes);
    }
}
