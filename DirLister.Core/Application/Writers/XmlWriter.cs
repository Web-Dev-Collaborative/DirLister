﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sander.DirLister.Core.Application.Writers
{
	internal sealed class XmlWriter : BaseWriter
	{
		public XmlWriter(Configuration configuration, DateTimeOffset endDate) : base(configuration, endDate)
		{
		}

		protected internal override string Write(List<FileEntry> entries)
		{
			var folders = GroupByFolder(entries);

			var xRoot = new XElement(nameof(DirLister));
			xRoot.SetAttributeValue("TotalFiles", entries.Count);
			xRoot.SetAttributeValue("TotalSize", entries.Sum(x => x.Size));
			xRoot.SetAttributeValue("RunDate", DateTimeOffset.Now);

			var xFolders = new List<XElement>();
			foreach (var fileEntries in folders)
			{
				var xFolder = GetFolderElement(fileEntries);
				xFolders.Add(xFolder);
			}

			xRoot.Add(xFolders);

			var fileName = GetFilename(OutputFormat.Xml);
			xRoot.Save(fileName);

			return fileName;
		}

		private XElement GetFolderElement(IGrouping<string, FileEntry> entries)
		{
			var xfolder = new XElement("Directory");
			xfolder.SetAttributeValue("Name", entries.Key);
			xfolder.SetAttributeValue("FileCount", entries.Count());
			xfolder.SetAttributeValue("TotalSize", entries.Sum(x => x.Size));
			var files = new List<XElement>();
			foreach (var entry in entries)
				files.Add(GetFileElement(entry));

			xfolder.Add(files);

			return xfolder;
		}

		private XElement GetFileElement(FileEntry entry)
		{
			var file = new XElement("File");
			file.SetAttributeValue("Name", entry.Filename);

			if (Configuration.IncludeSize)
				file.SetAttributeValue(nameof(entry.Size), entry.Size);

			if (Configuration.IncludeFileDates)
			{
				file.SetAttributeValue(nameof(entry.Created), entry.Created.ToLocalTime());
				file.SetAttributeValue(nameof(entry.Modified), entry.Modified.ToLocalTime());
			}

			if (Configuration.IncludeMediaInfo && entry.MediaInfo != null)
			{
				var media = GetMediaInfo(entry);
				file.Add(media);
			}

			return file;
		}

		private static XElement GetMediaInfo(FileEntry entry)
		{
			void NonZeroInsert(XElement element, string name, int value)
			{
				if (value != 0)
					element.SetAttributeValue(name, value);
			}


			var media = new XElement(nameof(entry.MediaInfo));
			media.SetAttributeValue(nameof(entry.MediaInfo.MediaType), entry.MediaInfo.MediaType);

			if (entry.MediaInfo.Duration != TimeSpan.Zero)
				media.SetAttributeValue(nameof(entry.MediaInfo.Duration), entry.MediaInfo.Duration.TotalSeconds);

			NonZeroInsert(media, nameof(entry.MediaInfo.Height), entry.MediaInfo.Height);
			NonZeroInsert(media, nameof(entry.MediaInfo.Width), entry.MediaInfo.Width);
			NonZeroInsert(media, nameof(entry.MediaInfo.BitsPerPixel), entry.MediaInfo.BitsPerPixel);
			NonZeroInsert(media, nameof(entry.MediaInfo.AudioBitRate), entry.MediaInfo.AudioBitRate);
			NonZeroInsert(media, nameof(entry.MediaInfo.AudioChannels), entry.MediaInfo.AudioChannels);
			NonZeroInsert(media, nameof(entry.MediaInfo.AudioSampleRate), entry.MediaInfo.AudioSampleRate);

			return media;
		}
	}
}
