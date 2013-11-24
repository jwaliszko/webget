#webget

```
webget: get files from web
usage: webget [options]... [url]...

examples:
  webget -e mp3,aac http://www.astronomycast.com/archive/
  webget -e jpg,png http://www.reddit.com/r/space -r 10 -d space -gt 1mb -lt 20mb -t /space/\?count.*after -l

options:
  -e,  --extensions=LIST                         comma-separated list of accepted extensions
  -p,  --proxy-settings=(user:pass@)ip(:port)    proxy settings (if required)
  -d,  --save-directory=PATH                     download directory
  -r,  --recursion-depth=NUMBER                  max recursion depth - default: 0, infinity: -1
  -t,  --recursion-target=PATTERN                regex url pattern to direct recursive search
  -l,  --link-label                              replace resource name with link label if possible
  -u,  --user-agent=NAME                         User-Agent HTTP field spoof value,
                                                 e.g. "Links (0.96; Linux 2.4.20-18.7 i586)"
  -gt, --greater-than=NUMBER[kb|mb]              upper file size boundary, no extent means bytes
  -lt, --less-than=NUMBER[kb|mb]                 lower file size boundary, no extent means bytes
  -h,  --help                                    print this help
```
