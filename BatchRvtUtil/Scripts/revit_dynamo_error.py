
import clr
import System

DYNAMO_REVIT_MODULE_NOT_FOUND_ERROR_MESSAGE = "Could not load the Dynamo module! There must be EXACTLY ONE VERSION of Dynamo installed!"

def IsDynamoNotFoundException(exception):
  return (
      isinstance(exception, Exception)
      and
      e.message == DYNAMO_REVIT_MODULE_NOT_FOUND_ERROR_MESSAGE
    )

