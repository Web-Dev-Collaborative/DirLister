﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sander.DirLister.Core.Application.Writers
{
	internal sealed class CsvWriter : BaseWriter
	{
		private readonly StringBuilder _sb;

		internal CsvWriter(Configuration configuration, DateTimeOffset endDate) : base(configuration, endDate)
		{
			_sb = new StringBuilder();
		}

		protected internal override string Write(List<FileEntry> entries)
		{
			GetHeaderLine();

			foreach (var entry in entries) GetFileLine(entry);

			return WriteFile(_sb, OutputFormat.Csv);
		}


		private void GetHeaderLine()
		{
			_sb.Append(Quote("File"));

			if (Configuration.IncludeSize)
				_sb.Append($",{Quote(nameof(FileEntry.Size))}");

			if (Configuration.IncludeFileDates)
			{
				_sb.Append($",{Quote(nameof(FileEntry.Created))}");
				_sb.Append($",{Quote(nameof(FileEntry.Modified))}");
			}

			if (Configuration.IncludeMediaInfo)
			{
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.MediaType))}");
				_sb.Append($",{Quote($"{nameof(FileEntry.MediaInfo.Duration)}(seconds)")} ");
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.Height))}");
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.Width))}");
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.BitsPerPixel))}");
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.AudioBitRate))}");
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.AudioChannels))}");
				_sb.Append($",{Quote(nameof(FileEntry.MediaInfo.AudioSampleRate))}");
			}

			_sb.AppendLine();
		}

		private void GetFileLine(FileEntry entry)
		{
			_sb.Append(Quote(entry.Fullname));

			if (Configuration.IncludeSize)
				_sb.Append($",{Quote(entry.Size)}");

			if (Configuration.IncludeFileDates)
			{
				_sb.Append($",{Quote(entry.Created.ToLocalTime().ToString())}");
				_sb.Append($",{Quote(entry.Modified.ToLocalTime())}");
			}

			if (Configuration.IncludeMediaInfo && entry.MediaInfo != null)
			{
				_sb.Append($",{Quote(entry.MediaInfo.MediaType.ToString())}");
				_sb.Append($",{Quote(entry.MediaInfo.Duration.TotalSeconds)}");
				_sb.Append($",{Quote(entry.MediaInfo.Height)}");
				_sb.Append($",{Quote(entry.MediaInfo.Width)}");
				_sb.Append($",{Quote(entry.MediaInfo.BitsPerPixel)}");
				_sb.Append($",{Quote(entry.MediaInfo.AudioBitRate)}");
				_sb.Append($",{Quote(entry.MediaInfo.AudioChannels)}");
				_sb.Append($",{Quote(entry.MediaInfo.AudioSampleRate)}");
			}

			_sb.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string Quote<T>(T value)
		{
			return value == null || value.Equals(default(T))
				? "\"\""
				: FormattableString.Invariant($"\"{value.ToString()}\"");
		}
	}
}
