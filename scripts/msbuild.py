
import clr
import System
clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

from System.IO import Path, Directory

from System import Console, ConsoleColor, InvalidOperationException
from System.Diagnostics import Process, ProcessStartInfo

import sys

import get_process_output

IO_TIME_OUT_IN_MS = 5 * 60 * 10000 # 5 minutes

#VC_FOLDER_PATH = r"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC"
VC_FOLDER_PATH_COMMUNITY_2017 = r"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build"
VC_FOLDER_PATH_COMMUNITY_2019 = r"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build"
VC_FOLDER_PATHS = [VC_FOLDER_PATH_COMMUNITY_2017, VC_FOLDER_PATH_COMMUNITY_2019]
VC_VARS_ALL_FILENAME = "vcvarsall.bat"
MSBUILD_FILENAME = "msbuild.exe"

DEBUG_CONFIG = "Debug"
EXPERIMENTAL_CONFIG = "Experimental"
RELEASE_CONFIG = "Release"
EXPECTED_CONFIGS = [DEBUG_CONFIG, EXPERIMENTAL_CONFIG, RELEASE_CONFIG]

def StartCmdProcess(commandLine, workingDirectory=None):
  # NOTE: do not call Process.WaitForExit() until redirected streams have been entirely read from / closed.
  #       doing so can lead to a deadlock when the child process is waiting on being able to write to output / error
  #       stream and the parent process is waiting for the child to exit! See Microsoft's documentation for more info.
  # NOTE: if redirecting both output and error streams, one should be read asynchronously to avoid a deadlock where
  #       the child process is waiting to write to one of the streams and the parent is waiting for data from the other
  #       stream. See Microsoft's documentation for more info.
  psi = ProcessStartInfo('cmd.exe', '/S /C " ' + commandLine + ' "')
  psi.UseShellExecute = False
  psi.CreateNoWindow = True
  # TODO: does this need to be set? what should it be? what will it be by default?
  if workingDirectory is not None:
    psi.WorkingDirectory = workingDirectory
  psi.RedirectStandardInput = False
  psi.RedirectStandardError = True # See notes above if enabling this alongside redirect output stream.
  psi.RedirectStandardOutput = True
  process = Process.Start(psi)
  return process

def ProcessOutput(outputLine):
  print outputLine
  return

def ProcessError(errorLine):
  previousColor = Console.ForegroundColor
  Console.ForegroundColor = ConsoleColor.Red
  print "  " + errorLine
  Console.ForegroundColor = previousColor
  return

def ShowUsage():
  print
  print "Usage:"
  print
  print "  " + "msbuild.py toolset build_file tasks configuration platform"
  print
  print "Example:"
  print
  print "  " + 'msbuild.py amd64 "C:\MySolution\MySolution.sln" Clean,Build Debug "Any CPU"'
  print
  return

argv = sys.argv
numOfArgs = len(argv)
if numOfArgs < 6 or numOfArgs > 7:
  print
  print "ERROR: Incorrect number of arguments specified!"
  ShowUsage()
  raise Exception("Incorrect number of arguments specified!")

toolset = argv[1]
buildFile = argv[2]
tasks = argv[3]
configuration = argv[4]
platform = argv[5]

DISABLE_POST_BUILD_EVENT = (numOfArgs > 6 and argv[6] == "NOPOSTBUILD")

if numOfArgs > 6 and not DISABLE_POST_BUILD_EVENT:
  raise Exception("Invalid argument!", argv[6])

print
print "Config:"
print
print "\tToolset: " + toolset
print "\tProject file: " + buildFile
print "\tTasks: " + tasks
print "\tConfiguration: " + configuration
print "\tPlatform: " + platform
print "\tDisable PostBuildEvent: " + str(DISABLE_POST_BUILD_EVENT)

print
print "Starting..."

if configuration not in EXPECTED_CONFIGS:
  raise Exception("Unexpected configuration specified: '" + configuration + "'")

VC_FOLDER_PATH = VC_FOLDER_PATHS.Where(lambda p: Directory.Exists(p)).LastOrDefault()

if VC_FOLDER_PATH is None:
  raise Exception("Could not locate MSBuild toolset (for vcvarsall.bat, etc.)")

COMMAND_PARTS = [
    r'"' + Path.Combine(VC_FOLDER_PATH, VC_VARS_ALL_FILENAME) + '"',
    toolset,
    "&&",
    MSBUILD_FILENAME,
    '"' + buildFile + '"',
    "/m", # enables parallel build
    "/t:" + tasks,
    "/p:Configuration=" + configuration,
    "/p:Platform=" + '"' + platform + '"',
  ]

if configuration == RELEASE_CONFIG:
  COMMAND_PARTS.extend([
      "/p:AllowedReferenceRelatedFileExtensions=none",
      "/p:DebugType=none"
    ])

if DISABLE_POST_BUILD_EVENT:
  COMMAND_PARTS.extend([
      "/p:PostBuildEvent="
    ])

COMMAND = str.Join(" ", COMMAND_PARTS)

print

process = StartCmdProcess(COMMAND)

outputLines, errorLines, completed = get_process_output.ReadProcessOutputAndErrorLines(
    process,
    ProcessOutput,
    ProcessError,
    IO_TIME_OUT_IN_MS
  )

try:
  process.Kill()
except Exception, InvalidOperationException:
  pass

if not completed:
  print
  print "Timed-out waiting for process!"
  sys.exit(1)

if process.ExitCode != 0:
  print
  print "Process returned non-zero error code!"
  sys.exit(process.ExitCode)

# NOTE: if an exception occurs above, a non-zero error code is returned so we don't have to explicitly set it in those cases.
sys.exit(0) # This is required to set the exit code. Setting Environment.ExitCode didn't seem to work!

