﻿from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base, declared_attr
from sqlalchemy import inspect
import json
import datetime

Base = declarative_base()
username="root"
server="localhost"
port=3306
database="!--databaseName--!"
engine = create_engine('mysql+mysqlconnector://%s@%s:%d/%s'%(username, server, port, database), echo=True)

def object_as_dict(obj):
	if isinstance(obj, list):
		objects = []
		for row in obj:
			objects.append(object_as_dict(row))
		return objects

    return {c: getattr(obj, c)
            for c in obj.__dict__.keys() if c[0]!='_'}

def dict_as_obj(args, obj):
	for arg in args:
		setattr(obj, arg, args[arg])
	return obj

def alchemyencoder(obj):
    """JSON encoder function for SQLAlchemy special classes."""
    if isinstance(obj, datetime.date):
        return obj.isoformat()
    else:
        return object_as_dict(obj)

def convertToJson(data):
	if isinstance(data, dict):
		return data
	objects = []
	for row in data:
		objects.append(object_as_dict(row))

	return json.dumps(objects, default = alchemyencoder)