using Microsoft.Web.WebView2.Core;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;

namespace DownloadComics.models
{
    public class AppState
    {
        private readonly ObservableCollection<Comic> Comics = [];

        private List<Track> _tracks = [];
        private readonly Lock _trackLock = new();
        public List<Track> Tracks
        {
            get
            {
                using (_trackLock.EnterScope())
                {
                    return [.. _tracks];
                }
            }
            set
            {
                _tracks = value;
            }
        }

        public void AddTrack(Track track)
        {
            using (_trackLock.EnterScope())
            {
                _tracks.Add(track);
            }
        }

        public void RemoveTrack(Track track)
        {
            using (_trackLock.EnterScope())
            {
                _tracks.Remove(track);
            }
        }

        public Track? GetTrackByUrl(string url)
        {
            using (_trackLock.EnterScope())
            {
                return _tracks.FirstOrDefault(t => t.DownloadURL == url);
            }
        }

        public ObservableCollection<Comic> GetComics()
        {
            return Application.Current.Dispatcher.Invoke(() => Comics);
        }

        public Comic? GetComicByUrl(string url)
        {
            return Application.Current.Dispatcher.Invoke(() => Comics.FirstOrDefault(comic => comic.URL == url));
        }
        public void AddComic(Comic comic)
        {
            Application.Current.Dispatcher.Invoke(() => Comics.Add(comic));
        }
        public void RemoveComic(Comic comic)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Comics.Remove(comic);
            });
        }

        public void ClearComics()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Comics.Clear();
            });
        }

        public long[] GetComicsId()
        {
            return [.. Application.Current.Dispatcher.Invoke(() => Comics.Select(c => c.UUID))];
        }
    }
}
