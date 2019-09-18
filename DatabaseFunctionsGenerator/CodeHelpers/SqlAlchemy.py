from sqlalchemy import *
from sqlalchemy.ext.declarative import declarative_base, declared_attr
Base = declarative_base()
username="root"
server="localhost"
port=3306
database="!--databaseName--!"
engine = create_engine('mysql+mysqlconnector://%s@%s:%d/%s'%(username, server, port, database), echo=True)
