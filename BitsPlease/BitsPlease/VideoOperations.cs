using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace BitsPlease
{
  class VideoOperations
  {
    private static string FFMPEG = "ffmpeg.exe";
    private static string BASEARGS = "-y ";
    private static string FFPROBE = "ffprobe.exe";
    

    public static void PerformCrop(
      string inputFile,
      string outputFile,
      uint x,
      uint y,
      uint width,
      uint height)
    {
      
    }


    Process FfmpegProcess()
    {
      Process process = new Process();
      process.StartInfo.FileName = FFMPEG;
      process.StartInfo.Arguments = BASEARGS;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.UseShellExecute = false;


      return process;
    }

  }
}
