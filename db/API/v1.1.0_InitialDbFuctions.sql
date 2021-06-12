CREATE OR REPLACE FUNCTION FindRowNumber
(pMessageId INTEGER, pAliasId INTEGER)
RETURNS TABLE ("RowNum" BIGINT)
LANGUAGE plpgsql    
AS $$
BEGIN
	RETURN QUERY SELECT rn.rownum
	FROM messages m 
	LEFT JOIN (
		SELECT m.id, ROW_NUMBER () OVER (ORDER BY date DESC) rownum 
		FROM messages m
		LEFT JOIN aliases_members am ON m.contact_id = am.contact_id
		LEFT JOIN aliases a ON am.alias_id = a.id
		WHERE a.id = pAliasID
	) rn ON rn.id = m.id 
	WHERE m.id = pMessageId;
END;
$$;

CREATE OR REPLACE FUNCTION DeleteMessagesWithImportId
(pImportId INTEGER)
RETURNS VOID
LANGUAGE plpgsql    
AS $$
BEGIN
	DELETE FROM messages WHERE import_id = pImportId;
END;
$$;

CREATE OR REPLACE FUNCTION DeleteEmptyContacts
(pAppUserId INTEGER)
RETURNS VOID
LANGUAGE plpgsql    
AS $$
BEGIN
	DELETE FROM aliases_members WHERE contact_id NOT IN
	(
		SELECT DISTINCT contact_id FROM messages
	);
	DELETE FROM contacts WHERE id NOT IN
	(
		SELECT DISTINCT contact_id FROM messages
	) AND app_user_id = pAppUserId;
	
	DELETE FROM aliases a WHERE a.internal = FALSE AND
	(
		SELECT COUNT(*) FROM aliases_members am WHERE am.alias_id = a.id
	) < 2;
	DELETE FROM aliases a WHERE a.internal = TRUE AND
	(
		SELECT COUNT(*) FROM aliases_members am WHERE am.alias_id = a.id
	) = 0;
END;
$$;