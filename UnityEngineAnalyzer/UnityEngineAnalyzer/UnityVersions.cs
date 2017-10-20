namespace UnityEngineAnalyzer
{
    public class UnityVersionSpan
    {
        public UnityVersion First { get; set; }
        public UnityVersion Last { get; set; }

        public UnityVersionSpan (UnityVersion first, UnityVersion last)
        {
            First = first;
            Last = last;
        }
    }

    public enum UnityVersion
    {
        NONE = -1,
        UNITY_1_0 = 1000,
        UNITY_2_0 = 2000,
        UNITY_3_0 = 3000,
        UNITY_3_5 = 3500,
        UNITY_4_0 = 4000,
        UNITY_4_1 = 4100,
        UNITY_4_2 = 4200,
        UNITY_4_3 = 4300,
        UNITY_4_4 = 4400,
        UNITY_4_5 = 4500,
        UNITY_4_6 = 4600,
        UNITY_4_7 = 4700,
        UNITY_5_0 = 5000,
        UNITY_5_1 = 5100,
        UNITY_5_2 = 5200,
        UNITY_5_3 = 5300,
        UNITY_5_4 = 5400,
        UNITY_5_5 = 5500,
        UNITY_5_6 = 5600,
        UNITY_2017_0 = 6000,
        UNITY_2017_1 = 6100,
        UNITY_2017_2 = 6200,
        UNITY_2017_3 = 6300,
        LATEST = 99999,
    }
}