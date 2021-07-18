local cursor = 0
local values = {}
local match = @key
local count = @value
local result = {}
local done = false
-- �o�Ӱj��|�g�J100��key
for i=1,100 do
    redis.call("set", "test:"..i, i+1)
end

-- �I�s SCAN, �è̾ڶǤJ��key pattern�P�C���d�ߵ���, �v��Ū���ŦX����key�s�Jtable
repeat
    local searchResult = redis.call("SCAN", cursor, "MATCH", match, "COUNT", count)
    cursor = searchResult[1];
    values = searchResult[2];
    for i, val in ipairs(values) do
        table.insert(result, val)
    end
    if cursor == "0" then
        done = true;
    end
until done

--�Ƨǫ�^�ǵ��G
table.sort(result)
return result