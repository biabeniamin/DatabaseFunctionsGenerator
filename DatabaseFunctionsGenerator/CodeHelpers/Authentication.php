<?php
header('Access-Control-Allow-Origin: *'); 
header('Access-Control-Allow-Headers: *'); 
header('Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS');
$_POST = json_decode(file_get_contents('php://input'), true);
require_once 'Models/Token.php';
require_once 'DatabaseOperations.php';
require_once 'Helpers.php';
require_once 'TokenUsers.php';
require_once 'Tokens.php';

function CheckToken($database, $value)
{
	$isAuthorized = 1;

	$tokens = GetTokensByValue($database, 
		$value
	);

	if(!$tokens[0]->tokenId)
		$isAuthorized = 0;

	$timestamp = gmdate("M d Y H:i:s", time())-strtotime($tokens[0]->lastUpdate);
	$elapsedHours = $timestamp / 60 / 60;
	if($elapsedHours > 1)
		$isAuthorized = 0;

	$ip = $_SERVER['REMOTE_ADDR'];
	if($ip != $tokens[0]->address)
		$isAuthorized = 0;

	if(!$isAuthorized)
		DeleteToken($database, $tokens[0]->tokenId);

	if($isAuthorized)
	{
		$now = gmdate("Y-m-d H:i:s");
		$tokens[0]->lastUpdate = $now;
		UpdateToken($database, $tokens[0]);
	}

	return (["isAuthorized" => $isAuthorized, "value" => $value]);
}

if(!CheckGetParameters(["token"]))
{
    if(CheckGetParameters(["cmd"]))
    {
        if("addToken" == $_GET["cmd"])
        {        
            if(CheckPostParameters([
                'username',
			    'password'
            ]))
            {
                $database = new DatabaseOperations();
                $users = GetTokenUsersByUsernamePassword($database, 
				    $_POST["username"],
				    $_POST["password"]
			    );

                if(!$users[0]->tokenUserId)
                    die(json_encode( GetEmptyToken()));

                $ip = $_SERVER['REMOTE_ADDR'];
                $now = gmdate("Y-m-d H:i:s");
                $guid = GUID();
                $token = new Token(
                    $users[0]->tokenUserId,//TokenUserId
                    $guid,//Value
                    $ip,//Address
                    $now//LastUpdate
                );
                
                die(json_encode(AddToken($database, $token)));
            }

        }
    }
    die(json_encode(["error"=>"Token was not provided."]));
}

$database = new DatabaseOperations();
$result = CheckToken($database, $_GET["token"]);
        
if(!$result['isAuthorized'])
    die(json_encode(["error"=>"Wrong token."]));

if("checkToken" == $_GET["cmd"])
    die(json_encode(["status"=>"ok"]));

?>