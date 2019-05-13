using System;
using System.Collections.Generic;
using Orpheus.Models;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public abstract class MpdServerBase
    {
        public virtual void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null) { }
        public virtual string ConnectionAsString { get; }
        public virtual void AddId(string uri) { }
        public virtual void AddId(string uri, string position) { }
        public virtual void DeleteId(string songId) { }
        public virtual void DisableOutput(string outputId) { }
        public virtual void EnableOutput(string outputId) { }
        public virtual void FilelistInfo(Action<MpdFileSystem> callback) { }
        public virtual void Next() { }
        public virtual void Outputs(Action<List<MpdOutput>> callback) { }
        public virtual void PlayId(string songId) { }
        public virtual void PlaylistInfo(Action<MpdPlaylist> callback) { }
        public virtual void Previous() { }
        public virtual void Random(string state) { }
        public virtual void Repeat(string state) { }
        public virtual void SeekCur(string time) { }
        public virtual void Stats(Action<MpdStats> callback) { }
        public virtual void Status(Action<MpdStatus> callback) { }
        public virtual void Stop() { }
        public virtual void Update() { }
    }
}