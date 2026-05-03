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

        //public ObservableCollection<Comic> GetComics()
        //{
        //    return Application.Current.Dispatcher.Invoke(() => Comics);
        //}

        //public Comic? GetComicByUrl(string url)
        //{
        //    return Application.Current.Dispatcher.Invoke(() => Comics.FirstOrDefault(comic => comic.URL == url));
        //}
        //public void AddComic(Comic comic)
        //{
        //    Application.Current.Dispatcher.Invoke(() => Comics.Add(comic));
        //}
        //public void RemoveComic(Comic comic)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        Comics.Remove(comic);
        //    });
        //}

        //public void ClearComics()
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        Comics.Clear();
        //    });
        //}

        //public long[] GetComicsId()
        //{
        //    return [.. Application.Current.Dispatcher.Invoke(() => Comics.Select(c => c.UUID))];
        //}
    }
}
