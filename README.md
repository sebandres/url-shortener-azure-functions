# url-shortener-azure-functions
A Url shortener built with azure functions

Consists of 2 functions:

## get-url
Method: `GET`
Format: `urlshortener/{hash}` where `{hash}` is the short url.
If the hash exists then it returns a redirect code with the redirect original url.
If hash does not exist then returns 404.

## post-new-url
Method: `POST`
Format: `urlshortener/new`
Payload: 
```
{
    OriginalUrl: 'http://www.google.com'
}
```
Returns:
```
{
    "ShortUrl": "c7e4",
    "OriginalUrl": "http://google.com",
    "Errors": null
}
```