using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Services
{
    public static class CrashTracker
    {
        public static void Track(Exception exception, object objectToTrack = null)
        {
            try
            {
                if (objectToTrack != null)
                {
                    IDictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("objectType", objectToTrack.GetType().ToString());
                    foreach (var item in objectToTrack.GetType().GetProperties())
                    {
                        var v = item.GetValue(objectToTrack);
                        if (v != null)
                            dict.Add(item.Name, v.ToString());
                    }
                    Crashes.TrackError(exception, dict);
                    return;
                }

                Crashes.TrackError(exception);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

        }

        public static void CreateTestCrash()
        {
            Crashes.GenerateTestCrash();
        }

    }
}
