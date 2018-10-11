<?php

$_POST = json_decode(file_get_contents('php://input'), true);

function CheckParameter($list, $parameter)
{
	if(!isset($list[$parameter]) || $list[$parameter] == "")
    {
        return false;
    }
	return true;
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
			return false;
		}
    }

    return true;
}

?>