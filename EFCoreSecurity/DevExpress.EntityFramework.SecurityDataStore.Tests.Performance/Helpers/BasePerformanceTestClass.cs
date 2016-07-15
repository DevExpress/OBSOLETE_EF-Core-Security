using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance {
    public class BasePerformanceTestClass {
        protected Dictionary<TestType, double> timeDifferences;
        protected double GetTimeDifference(TestType testType) {
            if(timeDifferences == null)
                return 0;

            double resultValue;
            bool isOK = timeDifferences.TryGetValue(testType, out resultValue);
            if(!isOK)
                resultValue = 0;
            return resultValue;
        }
        protected void SetTimeDifference(TestType testType, double time) {
            if(timeDifferences == null)
                timeDifferences = new Dictionary<TestType, double>();

            timeDifferences[TestType] = time;
        }

        protected string GetTimeDifferenceErrorString(double actual, double nominal) {
            return "actual: " + actual.ToString() + " ms, nominal: " + nominal + " ms";
        }

        protected string GetDebugTimeString(double securedTime, double nativeTime) {
            return "our: " + securedTime.ToString() + " ms, native: " + nativeTime.ToString() + " ms";
        }
    }
}
