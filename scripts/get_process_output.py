
import clr
import System
clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)
from System.Collections.Generic import Dictionary
from System.Threading.Tasks import Task

class StreamData(object):
  def __init__(self, stream):
    self.stream = stream
    self.lines = []
    return
  
  def AddLine(self, line):
    self.lines.append(line)

  def GetLines(self):
    return self.lines

  def GetStream(self):
    return self.stream

def ReadProcessOutputAndErrorLines(process, processOutput, processError, timeOutInMs=-1):
  completed = False
  outputData = StreamData(process.StandardOutput)
  errorData = StreamData(process.StandardError)
  task2stream = Dictionary[Task, StreamData]()
  task2stream[outputData.GetStream().ReadLineAsync()] = outputData
  task2stream[errorData.GetStream().ReadLineAsync()] = errorData
  while True:
    tasks = task2stream.Keys.ToArray[Task]()
    index = Task.WaitAny(tasks, timeOutInMs)
    if index == -1: # timed out!
      completed = False
      break
    task = tasks[index]
    line = task.Result
    streamData = task2stream[task]
    task2stream.Remove(task) # must remove the completed task.
    if line is not None:
      streamData.AddLine(line)
      if streamData == outputData:
        processOutput(line)
      elif streamData == errorData:
        processError(line)
      else:
        raise Exception("Unexpected program state!")
      task2stream[streamData.GetStream().ReadLineAsync()] = streamData

    if not task2stream.Any():
      completed = True
      break
  return outputData.GetLines(), errorData.GetLines(), completed

