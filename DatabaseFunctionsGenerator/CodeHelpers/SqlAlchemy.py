from sqlalchemy import *
from sqlalchemy.ext.declarative import declarative_base, declared_attr
from sqlalchemy import inspect
Base = declarative_base()
username="root"
server="localhost"
port=3306
database="!--databaseName--!"
engine = create_engine('mysql+mysqlconnector://%s@%s:%d/%s'%(username, server, port, database), echo=True)

def object_as_dict(obj):
    return {c.key: getattr(obj, c.key)
            for c in inspect(obj).mapper.column_attrs}

