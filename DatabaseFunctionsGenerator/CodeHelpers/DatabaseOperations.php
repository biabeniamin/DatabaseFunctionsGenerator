<?php

class DatabaseOperations
{
    var $connection;

    function DatabaseOperations()
    {
        $this->OpenConnection();
    }

    function OpenConnection()
    {
        //$this->connection = new mysqli("localhost", "avramian_biabeni", "Acetronic1","avramian_hs");
        //$this->connection = new mysqli("localhost", "pagespee_biabeniamin", "Acetronic1","pagespee_hard");
        //$this->connection = new mysqli("localhost", "pagespee_biabeniamin", "Acetronic1","pagespee_hs18");
        //$this->connection = new mysqli("localhost", "root", "","generator");
        $this->connection = new mysqli("localhost", "avramian_codefor", "Acetronic1","avramian_codeforgood18");
        if ($this->connection->connect_error)
        {
            die("Connection failed: ".$this->connection->connect_error);
        }
    }

    function ExecuteSql($Sql)
    {
        if ($this->connection->query($Sql) === TRUE)
        {
            echo "New record created successfully<br>";
            return $this->GetLastInsertedId();
        } else {
            echo "Error: " . $sql . "<br>" . $this->connection->error;
        }
    }
    
    function ExecuteSqlWithoutWarning($Sql)
    {
        if ($this->connection->query($Sql) === TRUE)
        {
            return $this->GetLastInsertedId();
        } 
        return 0;
    }
	
	function GetLastInsertedId()
	{
		return $this->connection->insert_id;
	}
    
    
    function ReadOneValue($Sql)
    {
        $result = $this->connection->query($Sql);
        /*if ($result->num_rows <= 50)
            die("empty table");*/
        $row=$result->fetch_assoc();
       return $row;
    }

    function ReadData($Sql)
    {
        $result = $this->connection->query($Sql);
        $data = [];

        for($i=0;$i<$result->num_rows;$i++)
        {
            $row=$result->fetch_assoc();
            $data[$i] = $row;

        }

        return $data;
    }
}

?>