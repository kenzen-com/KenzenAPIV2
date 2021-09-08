using System;
using System.Collections.Generic;
using System.Text;

namespace KenzenAPI
{
    [Serializable]
    public class ProcessResult
    {
        public Object ObjectProcessed;
        public Exception Exception;
        public string Result = "";
    }
}
