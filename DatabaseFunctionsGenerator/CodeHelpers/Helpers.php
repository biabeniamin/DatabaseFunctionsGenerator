<?php

$_POST = json_decode(file_get_contents('php://input'), true);

function CheckParameter($list, $parameter)
{
	if(isset($list[$parameter]))
    {
        if($list[$parameter] != "" || $list[$parameter] == 0)
        {
            return true;
        }
    }
	return false;
}

function CheckGetParameters($list)
{
    foreach($list as $parameter)
    {
        if(!CheckParameter($_GET, $parameter))
		{
			return false;
		}
    }

    return true;
}

function CheckPostParameters($list)
{
    foreach($list as $parameter)
    {
        if(!CheckParameter($_POST, $parameter))
		{
            file_put_contents("object.log",$parameter." empty");
			return false;
		}
    }

    return true;
}

?>