using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Reporting
{
    public class RptHeartRate :ReportingBase
    {
        #region Vars

        int _TeamID;
        int _StepRate_1min;
        decimal _CBTPostRateLim_1min;
        decimal _HeartRateAvg5_1min;
        string _GMT;
        int _StepCount_1min;

        #endregion Vars

        #region Get/Sets

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }
        public int StepRate_1min
        {
            get { return (_StepRate_1min); }
            set { _StepRate_1min = value; }
        }

        public decimal CBTPostRateLim_1min
        {
            get { return (_CBTPostRateLim_1min); }
            set { _CBTPostRateLim_1min = value; }
        }

        public decimal HeartRateAvg5_1min
        {
            get { return (_HeartRateAvg5_1min); }
            set { _HeartRateAvg5_1min = value; }
        }

        public string GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        public int StepCount_1min
        {
            get { return (_StepCount_1min); }
            set { _StepCount_1min = value; }
        }


        #endregion Get/Sets
    }
}
