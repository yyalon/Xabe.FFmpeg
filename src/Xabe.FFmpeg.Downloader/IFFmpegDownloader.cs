using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    /// <summary>Downloads the Latest Version of FFmpeg</summary>
    public interface IFFmpegDownloader
    {
        /// <summary>
        ///     Do the download and install FFmpeg
        /// </summary>
        /// <param name="path">FFmpeg executables destination directory</param>
        Task GetLatestVersion(string path);
    }
}
