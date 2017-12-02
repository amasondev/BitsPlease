using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace BitsPlease
{
  public class VideoOperations
  {
    public static string FFMPEG = "ffmpeg.exe";
    public static string BASEARGS = "-y ";
    public static string FFPROBE = "ffprobe.exe";


    public static void PerformCrop(
      Window window,
      string inputPath,
      string outputPath,
      uint x,
      uint y,
      uint width,
      uint height)
    {
      // Lock the window during the operation
      window.IsEnabled = false;

      // TODO: Create a progress window

      Process cropProcess;

      if (!FF_Crop(out cropProcess, inputPath, outputPath, x, y, width, height))
      {
        Console.WriteLine("Error starting FF_Crop");
        return;
      }

      StreamReader streamreader = cropProcess.StandardError;
      string line;
      while ((line = streamreader.ReadLine()) != null)
        Console.WriteLine(line);

      cropProcess.WaitForExit();
      cropProcess.Close();

      // Unlock the window
      window.IsEnabled = true;
    }
    
    private static bool FF_Crop(
      out Process process,
      string inputPath,
      string outputPath,
      uint x,
      uint y,
      uint width,
      uint height
      )
    {
      process = FFmpegProcess();
      process.StartInfo.Arguments +=
        "-i \"" + inputPath + "\" " +
        "-vf crop=" + width.ToString() + ":" + height.ToString() + ":" +
        x.ToString() + ":" + y.ToString() +
        "-vcodec copy -acodec copy " +
        outputPath;

      Console.WriteLine("LAUNCHING: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

      return process.Start();
    }

    public static void PerformTrim(
            Window window,
            string inputPath,
            string outputPath,
            string start,
            string duration)

    {
      Process trimProcess;
      if (!FF_Trim(out trimProcess, inputPath, outputPath, start, duration))
      {
        Console.WriteLine("Error starting FF_Trim");
        return;
      }

      StreamReader streamreader = trimProcess.StandardError;
      string line;
      while ((line = streamreader.ReadLine()) != null)
        Console.WriteLine(line);

      trimProcess.WaitForExit();
      trimProcess.Close();


      window.IsEnabled = true;
    }

    private static bool FF_Trim(
        out Process process,
        string inputPath,
        string outputPath,
        string start,
        string duration)
    {
      process = FFmpegProcess();
      process.StartInfo.Arguments +=
      "-y -i \"" + inputPath + "\" -ss " + start + " -t " + duration + " " + outputPath;

      Console.WriteLine("LAUNCHING: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

      return process.Start();
    }


    private static Process FFmpegProcess()
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


    //fix line 169
    /*
    public static void PerformConvert(
        Window window,
        string inputPath,
        string outputPath,
        string format
        )
        {
            Process convertProcess;
            if(!FF_Convert (out convertProcess, inputPath, outputPath,format))
            {
                Console.WriteLine("Error starting FF_Convert");
                return;
            }

            StreamReader streamreader = convertProcess.StandardError;
            string line;
            while ((line = streamreader.ReadLine()) != null)
                Console.WriteLine(line);

            convertProcess.WaitForExit();
            convertProcess.Close();


            window.IsEnabled = true;
        }

        private static bool FF_Convert(
        out Process process,
        string inputPath,
        string outputPath,
        string format)
        {
            process = FFmpegProcess();
            process.StartInfo.Arguments +=
            "-y -i \"" + inputPath + "\" -ss " + start + " -t " + duration + " " + outputPath; //Must be chanched!!

            Console.WriteLine("LAUNCHING: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

            return process.Start();
        }

    */
    }
}
