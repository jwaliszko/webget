#webget

```
webget: get files from websites
usage: webget [options]... [url]...

examples:
  webget -e mp3,aac http://www.astronomycast.com/archive/
  webget -e jpg http://www.reddit.com/r/space -r 20 -d space -gt 500kb -t "\?count.*after" -n "mars|moon" -l

options:
  -e,  --extensions=LIST                         comma-separated list of accepted extensions
  -p,  --proxy-settings=(user:pass@)ip(:port)    proxy settings (if required)
  -d,  --save-directory=PATH                     download directory
  -r,  --recursion-depth=NUMBER                  max recursion depth - default: 0, infinity: -1
  -t,  --recursion-target=REGEX                  regex url pattern to direct recursive search
  -l,  --link-label                              replace resource name with link label if possible
  -n,  --name-filter=REGEX                       regex resource name to be downloaded
  -o,  --request-timeout=NUMBER                  request timeout in miliseconds before abort
  -u,  --user-agent=NAME                         User-Agent HTTP field spoof value,
                                                 e.g. "Links (0.96; Linux 2.4.20-18.7 i586)"
  -gt, --greater-than=NUMBER[kb|mb]              upper file size boundary, no extent means bytes
  -lt, --less-than=NUMBER[kb|mb]                 lower file size boundary, no extent means bytes
  -h,  --help                                    print this help
```
