def checkArguments(request, arguments):
	print(request)
	print('data' in request)
	if 'data' not in request:
		return False

	for arg in arguments:
		if arg not in request['data']:
			return False
	return True