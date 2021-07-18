local cursor = 0
local values = {}
local match = @key
local count = @value
local result = {}
local done = false
-- 這個迴圈會寫入100個key
for i=1,100 do
    redis.call("set", "test:"..i, i+1)
end

-- 呼叫 SCAN, 並依據傳入的key pattern與每次查詢筆數, 逐次讀取符合條件的key存入table
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

--排序後回傳結果
table.sort(result)
return result