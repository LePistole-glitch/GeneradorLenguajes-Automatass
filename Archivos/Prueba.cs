#include <stdio.h>
#include <conio.h>
#include <math.h>
#include <stdlib>
#include <graphics.h>

int i,j,maximo;
void main()
{
     printf("maximo= ");
     scanf("%d",&maximo);
    for(i=0;i!=maximo; i++)
    {
         j=i;
         while(j!=0)
         {
              if(j%2==0) 
              {
                    printf("+");
              }
              /*else
              {
                   printf("-");
              }*/
          j--;
         }
         printf(".");
    } 
}