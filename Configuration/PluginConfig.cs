using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using ChzzkChat.UI;
using UnityEngine;
using System.ComponentModel;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ChzzkChat.Configuration
{
    struct Request
    {
        public string SongCode { get; set; }
        public string SongName { get; set; }

        public Request(string songCode, string songName)
        {
            SongCode = songCode;
            SongName = songName;
        }

        override
        public string ToString()
        {
            return $@"{SongCode} {this.SongName}";
        }
    }

    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public virtual string ChannelId { get; set; } = "";

        public virtual bool RequestQueOpen { get; set; } = true;
        public virtual string RequestCommand { get; set; } = "";
        public virtual int RequestMaxCount { get; set; } = 5;

        [NonNullable]
        [UseConverter(typeof(ListConverter<Request>))]
        public virtual List<Request> RequestList { get; set; } = new List<Request>();

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.

        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            UIManager.Instance?.UpdateList();
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
}
