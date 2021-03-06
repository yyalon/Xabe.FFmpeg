﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IAudioStream" />
    public class AudioStream : IAudioStream, IFilterable
    {
        private readonly Dictionary<string, string> _audioFilters = new Dictionary<string, string>();
        private string _bitsreamFilter;
        private string _reverse;
        private string _seek;
        private string _split;
        private string _sampleRate;
        private string _channels;
        private string _bitrate;
        private string _codec;

        internal AudioStream()
        {

        }

        /// <inheritdoc />
        public IAudioStream Reverse()
        {
            _reverse = "-af areverse ";
            return this;
        }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(BuildAudioCodec());
            builder.Append(_bitsreamFilter);
            builder.Append(_sampleRate);
            builder.Append(_channels);
            builder.Append(_bitrate);
            builder.Append(_reverse);
            builder.Append(_split);
            return builder.ToString();
        }

        /// <inheritdoc />
        public string BuildInputArguments()
        {
            return _seek;
        }

        /// <inheritdoc />
        public string BuildAudioCodec()
        {
            if (_codec != null)
                return $"-c:a {_codec.ToString()} ";
            else
                return string.Empty;
        }

        /// <inheritdoc />
        public IAudioStream Split(TimeSpan startTime, TimeSpan duration)
        {
            _split = $"-ss {startTime.ToFFmpeg()} -t {duration.ToFFmpeg()} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream CopyStream()
        {
            _audioFilters["-c:v copy"] = string.Empty;
            return this;
        }

        /// <inheritdoc />
        public StreamType StreamType => StreamType.Audio;

        /// <inheritdoc />
        public IAudioStream SetChannels(int channels)
        {
            _channels = $"-ac:{Index} {channels} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetBitstreamFilter(BitstreamFilter filter)
        {
            return SetBitstreamFilter($"{filter}");
        }

        /// <inheritdoc />
        public IAudioStream SetBitstreamFilter(string filter)
        {
            _bitsreamFilter = $"-bsf:a {filter} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetBitrate(long bitRate)
        {
            _bitrate = $"-b:a:{Index} {bitRate} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetSampleRate(int sampleRate)
        {
            _sampleRate = $"-ar:{Index} {sampleRate} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream ChangeSpeed(double multiplication)
        {
            _audioFilters["atempo"] = $"{string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", GetAudioSpeed(multiplication))}";
            return this;
        }

        private string GetAudioSpeed(double multiplication)
        {
            if (multiplication < 0.5 || multiplication > 2.0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplication), "Value has to be greater than 0.5 and less than 2.0.");
            }

            return $"{string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", multiplication)} ";
        }

        /// <inheritdoc />
        public IAudioStream SetCodec(AudioCodec codec)
        {
            string input = codec.ToString();
            if (codec == AudioCodec._4gv)
            {
                input = "4gv";
            }
            else if (codec == AudioCodec._8svx_exp)
            {
                input = "8svx_exp";
            }
            else if (codec == AudioCodec._8svx_fib)
            {
                input = "8svx_fib";
            }
            return SetCodec($"{input}");
        }

        /// <inheritdoc />
        public IAudioStream SetCodec(string codec)
        {
            _codec = codec;
            return this;
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string Codec { get; internal set; }

        /// <inheritdoc />
        public long Bitrate { get; internal set; }

        /// <inheritdoc />
        public int Channels { get; internal set; }

        /// <inheritdoc />
        public int SampleRate { get; internal set; }

        /// <inheritdoc />
        public string Language { get; internal set; }

        /// <inheritdoc />
        public int? Default { get; internal set; }

        /// <inheritdoc />
        public int? Forced { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { Path };
        }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public IAudioStream SetSeek(TimeSpan? seek)
        {
            if (seek.HasValue)
            {
                _seek = $"-ss {seek.Value.ToFFmpeg()} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<IFilterConfiguration> GetFilters()
        {
            if (_audioFilters.Any())
            {
                yield return new FilterConfiguration
                {
                    FilterType = "-filter:a",
                    StreamNumber = Index,
                    Filters = _audioFilters
                };
            }
        }
    }
}
