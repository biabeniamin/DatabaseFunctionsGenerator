class ValidationError(Exception):
    """Raised when there is a validation error.
    This is used for testing validation errors only.
    """
    pass

def validate_integer(key, value, isMandatory):		
	if isMandatory == False and not value:
		return value
	try:
		val = int(value)
	except:
		exception = ValidationError([])
		exception.errors = str(key) + ' must be an integer'
		raise exception
	return value
