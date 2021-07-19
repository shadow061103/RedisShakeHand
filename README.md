# RedisShakeHand
Redis with .Net Core

資料可以參考本人筆記
[Redis基本指令與安裝](https://hackmd.io/@gs9TPhYbSPCyczQit5ucew/BJeNgZv2d) 
[Redis進階用法](https://hackmd.io/@gs9TPhYbSPCyczQit5ucew/H15RTaMTu)
[Redis管理與持久化](https://hackmd.io/@gs9TPhYbSPCyczQit5ucew/SyXibtKad)
[.Net Core的Redis](https://hackmd.io/@gs9TPhYbSPCyczQit5ucew/SJrrmI6p_)

### 安裝套件
`Install-Package MessagePack -Version 2.2.113`
`Install-Package Microsoft.Extensions.Caching.StackExchangeRedis -Version 6.0.0-preview.6.21355.2`
`Install-Package StackExchange.Redis -Version 2.2.62`
`Install-Package Swashbuckle.AspNetCore -Version 6.1.4`(swagger使用)


### IDistributedCache
- IDistributedCache是 .Net Core內建的分散式快取，只能使用Get、Set、Remove、Refresh
- 有封裝StackExchange.Redis在內，僅使用到Hash的型別，也會幫你設定逾時時間，資料主要會存放在`data`裡面
- 資料必須轉成byte[]
- 一般沒特殊需求用來存放快取
### StackExchange.Redis
- 需要注入`IConnectionMultiplexer`
- 可以操作Redis多種型別，可以用Pub/Sub，Lua script，較為彈性
