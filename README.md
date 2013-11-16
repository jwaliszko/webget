#webget

```
webget: get files from web
usage: webget [options]... [url]...

example: webget -e mp3,ogg http://www.astronomycast.com/archive/

options:
  -e, --extensions=LIST                         comma-separated list of accepted extensions
  -p, --proxy-settings=(user:pass@)ip(:port)    proxy settings if required
  -s, --save-directory=PATH                     download directory
  -r, --recursion-depth=LEVEL                   max recursion depth - default: 0, infinity: -1
  -u, --user-agent=NAME                         User-Agent HTTP field spoof value,
                                                e.g. "Links (0.96; Linux 2.4.20-18.7 i586)"
  -h, --help                                    print this help
```
