#-------------------------------------------------------------------------------
# Name:        module2
# Purpose:
#
# Author:      Raga
#
# Created:     26/02/2013
# Copyright:   (c) Raga 2013
# Licence:     <your licence>
#-------------------------------------------------------------------------------
import json
from pprint import pprint
import re
import numpy as np
from collections import defaultdict
from sets import Set
import scipy.spatial.distance
from scipy import stats
import operator
#############################Function used to build Data############################################
path ='C:\Users\Raga\Google Drive\CoursesSpring 2013\Data Science\Project\Data\yelp_phoenix_academic_dataset.tgz\yelp_phoenix_academic_dataset\yelp_phoenix_academic_dataset\yelp_academic_dataset_review.json'
#path='C:\Users\Raga\Google Drive\CoursesSpring 2013\Data Science\HW4\reveiws.txt'
def buildData(path):
    json_data=open(path)
    data = []
    count=0
    with open(path) as f:
     for line in f:
        count=count+1
        line=line.lower()
        data.append(json.loads(line))
    return data
print 'Building File'
data=buildData(path)
print 'Length ', len(data)
review=[]
counter=1
review_5=[]
review_4=[]
review_3=[]
review_2=[]
review_1=[]
for line in data:
   if 'text' in line:
    try:
     if line['stars']==5:
      review_5.append(line['text'])
      continue
     if line['stars']==4:
      review_4.append(line['text'])
      continue
     if line['stars']==3:
      review_3.append(line['text'])
      continue
     if line['stars']==2:
      review_2.append(line['text'])
      continue
     if line['stars']==1:
      review_1.append(line['text'])

    except:
     print 'Error in ',line['text']
file=open('reviews_5stars.txt','w')
counter=0
for line in review_5:
    line=line.encode('utf-8')
    file.write(line)
    file.write('\n')
file.close

file=open('reviews_4stars.txt','w')
counter=0
for line in review_4:
    line=line.encode('utf-8')
    file.write(line)
    file.write('\n')
file.close

file=open('reviews_3stars.txt','w')
counter=0
for line in review_3:
    line=line.encode('utf-8')
    file.write(line)
    file.write('\n')
file.close

file=open('reviews_2stars.txt','w')
counter=0
for line in review_2:
    line=line.encode('utf-8')
    file.write(line)
    file.write('\n')
file.close

file=open('reviews_1stars.txt','w')
counter=0
for line in review_1:
    line=line.encode('utf-8')
    file.write(line)
    file.write('\n')
file.close