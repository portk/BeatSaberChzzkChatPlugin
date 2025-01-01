using ChzzkChat.Configuration;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace ChzzkChat.Util
{
    internal class RequestListUtil
    {
        public static IList<CustomCellInfo> RequestListToCustomCellInfoList()
        {
            IList<CustomCellInfo> res = new List<CustomCellInfo>();

            foreach(Request item in PluginConfig.Instance.RequestList)
            {
                res.Add(new CustomCellInfo(item.ToString()));
            }

            return res;
        }
    }
}
