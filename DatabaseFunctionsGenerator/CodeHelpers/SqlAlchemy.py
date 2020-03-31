﻿from sqlalchemy import *
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
    return {c: getattr(obj, c)
            for c in obj.__dict__.keys() if c[0]!='_'}

def dict_as_obj(args, loc2, obj):
	loc = obj()
	for arg in args:
		setattr(loc, arg, args[arg])
	return loc 

def alchemyencoder(obj):
    """JSON encoder function for SQLAlchemy special classes."""
    if isinstance(obj, datetime.date):
        return obj.isoformat()
    else:
        return object_as_dict(obj)

def convertToJson(data):
	objects = []
	for row in data:
		objects.append(object_as_dict(row))

	return json.dumps(objects, default = alchemyencoder)