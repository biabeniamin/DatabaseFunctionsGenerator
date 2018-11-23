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
        $this->connection = new mysqli("localhost", "root", "","generator");
        //$this->connection = new mysqli("localhost", "avramian_codefor", "Acetronic1","avramian_codeforgood18");
        if ($this->connection->connect_error)
        {
            die("Connection failed: ".$this->connection->connect_error);
        }
    }

    function ExecuteSql($Sql, $Parameters)
    {
        $result = 0;

        $statement = $this->connection->prepare($Sql);

        foreach($Parameters as $parameter)
        {
            $statement->bind_param($parameter[0], $parameter[1]);
        }

        if ($statement->execute() === TRUE)
        {
            echo "New record created successfully<br>";
            return $this->GetLastInsertedId();
        } 
        else
        {
            echo "Error: " . $Sql . "<br>" . $this->connection->error;
        }
        
        $statement->close();

        return $result;


        if ($this->connection->query($Sql) === TRUE)
        {
            echo "New record created successfully<br>";
            return $this->GetLastInsertedId();
        } else {
            echo "Error: " . $Sql . "<br>" . $this->connection->error;
        }
    }
    
    function ExecuteSqlWithoutWarning($Sql, $Parameters)
    {
        $result = 0;

        $statement = $this->connection->prepare($Sql);

        foreach($Parameters as $parameter)
        {
            $statement->bind_param($parameter[0], $parameter[1]);
        }

        if ($statement->execute() === TRUE)
        {
            $result = 1;
        } 
        
        $statement->close();

        return $result;
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