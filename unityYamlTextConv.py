#!/usr/bin/env python

import sys
import yaml
import pprint

if len(sys.argv) < 2:
    sys.exit(-1)

def tag_unity3d_com_ctor(loader, tag_suffix, node):
    values = loader.construct_mapping(node, deep=True)
    if 'Prefab' in values:
        if 'm_Modification' in values['Prefab']:
            del values['Prefab']['m_Modification']
    return values

class UnityParser(yaml.parser.Parser):
    DEFAULT_TAGS = {u'!u!': u'tag:unity3d.com,2011'}
    DEFAULT_TAGS.update(yaml.parser.Parser.DEFAULT_TAGS)

class UnityLoader(yaml.reader.Reader, yaml.scanner.Scanner, UnityParser, yaml.composer.Composer, yaml.constructor.Constructor, yaml.resolver.Resolver):
    def __init__(self, stream):
        yaml.reader.Reader.__init__(self, stream)
        yaml.scanner.Scanner.__init__(self)
        UnityParser.__init__(self)
        yaml.composer.Composer.__init__(self)
        yaml.constructor.Constructor.__init__(self)
        yaml.resolver.Resolver.__init__(self)

UnityLoader.add_multi_constructor('tag:unity3d.com', tag_unity3d_com_ctor)
with open(sys.argv[1], 'r') as stream:
    docs = yaml.load_all(stream, Loader=UnityLoader)
    for doc in docs:
        pprint.pprint(doc, width=120, indent=1, depth=6)