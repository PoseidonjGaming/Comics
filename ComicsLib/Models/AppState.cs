using ComicsLib.Services;
using System.Collections.ObjectModel;

namespace ComicsLib.Models
{
    public class AppState
    {
        public ObservableCollection<Comic> Comics { get; set; } = [];

        private List<Track> _tracks = [];
        private readonly ReaderWriterLockSlim _trackLock = new();
        public List<Track> Tracks
        {
            get
            {
                _trackLock.EnterReadLock();
                try
                {
                    return [.. _tracks];
                }
                finally
                {
                    _trackLock.ExitReadLock();
                }
            }
            set
            {
                _tracks = value;
            }
        }

        public void AddTrack(Track track)
        {

            _trackLock.EnterWriteLock();
            try
            {
                _tracks.Add(track);
            }
            finally
            {
                _trackLock.ExitWriteLock();
            }
        }

        public void RemoveTrack(Track track)
        {
            _trackLock.EnterWriteLock();
            try
            {
                _tracks.Remove(track);
            }
            finally
            {
                _trackLock.ExitWriteLock();
            }
        }

        public Track? GetTrackByUrl(string url)
        {
            _trackLock.EnterReadLock();
            try
            {
                return _tracks.FirstOrDefault(t => t.DownloadURL == url);
            }
            finally
            {
                _trackLock.ExitReadLock();
            }
        }

        public void Init()
        {
            if (File.Exists(FileService.BackupFilePath))
                Comics = new(FileService.ReadFile<List<Comic>>(FileService.BackupFilePath) ?? []);

            if (File.Exists(FileService.TrackFilePath))
                Tracks = FileService.ReadFile<List<Track>>(FileService.TrackFilePath) ?? [];
        }
    }
}
