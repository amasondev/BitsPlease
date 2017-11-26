using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;

namespace BitsPlease
{
  /// <summary>
  /// Interaction logic for VideoDropWindow.xaml
  /// </summary>
  public class VideoDropWindow : Window
  {
    public VideoDropWindow()
    {
      AllowDrop = true;
      DragEnter += OnDragEnter;
      Drop += OnDragDrop;
    }

    protected static bool GetIsValidVideoPath(out string path, DragEventArgs e)
    {
      path = "";

      // All valid extensions the program will handle
      // TODO: Use ffprobe to determine if video is valid
      string[] validExts = {
        ".mp4",
        ".ogv",
        ".avi",
        ".wmv",
        ".mov",
        ".flv",
        ".webm",
        ".mkv",
        ".mpeg",
        ".mpeg2",
        ".mpeg4",
        ".mpg",
        ".mpg4",
        ".mpv",
        ".mpv2",
        ".264",
        ".x264",
        ".265",
        ".x265",
        ".xvid",
        ".dv"
        };

      // Get path from drag&drop event data
      string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
      // No file path found
      if (FileList.Length <= 0) return false;

      path = FileList[0];
      Console.WriteLine("Got path: " + path);

      string ext = Path.GetExtension(path).ToLower();
      Console.WriteLine("Extension is: " + ext);
      if (validExts.Contains(ext))
      {
        // File extension matches
        return true;
      }

      return false;
    }

    protected void OnDragEnter(object sender, DragEventArgs e)
    {
      Console.WriteLine("Begin OnDragEnter");
      string nullstr;
      bool fileIsValidVideo = GetIsValidVideoPath(out nullstr, e);

      // Set cursor
      e.Effects = (fileIsValidVideo) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    protected void OnDragDrop(object sender, DragEventArgs e)
    {
      Console.WriteLine("Begin OnDragDrop");
      try
      {
        Activate();
        string filepath;
        bool isValid = GetIsValidVideoPath(out filepath, e);

        if (!isValid) throw new Exception("Invalid video file.");

        // Let inherited windows handle the event once determined valid
        OnDropVideo(filepath);

      } catch (Exception ex)
      {
        // TODO: User friendly error messages
        MessageBox.Show(ex.Message);
      }
    }

    protected virtual void OnDropVideo(string filepath)
    {

    }
  
  }
}
