#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>

struct sockaddr_in serv_addr, cli_addr;

int listenfd, connfd, r, w, val, cli_addr_len;

unsigned short serv_port = 25020;
char serv_ip[] = "192.168.24.15";

char buff[128];

int main()
{
	bzero(&serv_addr, sizeof(serv_addr));
	
	serv_addr.sin_family = AF_INET;
	serv_addr.sin_port = htons(serv_port);
	inet_aton(serv_ip, (&serv_addr.sin_addr));
	
	printf("\nTCP ECHO SERVER.\n");
	
	if((listenfd = socket(AF_INET, SOCK_STREAM,0))< 0)
	{
		printf("\nSERVER ERROR: Cannot create socket\n");
		exit(1);
	}
	
	if((bind(listenfd,(struct sockaddr*)&serv_addr,sizeof(serv_addr)))<0)
	{
		printf("\nSERVER ERROR: Cannot bind\n");
		exit(1);
	}
	
	if((listen(listenfd,5))<0)
	{
		printf("\nSERVER ERROR: Cannot listen\n");
		close(listenfd);
		exit(1);
	}
	
	cli_addr_len = sizeof(cli_addr);
	for( ; ;)
	{
		printf("\nSERVER: Listening for clients... Press Ctrl+C to stop echo server\n");
		
		if((connfd = accept(listenfd,(struct sockaddr*)&cli_addr,&cli_addr_len)) < 0)
		{
			printf("\nSERVER ERROR: Cannot accept client connections\n");
			close(listenfd);
			exit(1);
		}
		printf("\nSERVER: Connection from client %s accepted.\n",inet_ntoa(cli_addr.sin_addr));
		
		if((r = read(connfd,buff,128))<0)
			printf("\nSERVER ERROR: Cannot receive message from client\n");
		else
		{
			buff[r] = '\0';
			
			if((w = write(connfd,buff,128))<0)
				printf("\nSERVER ERROR: Cannot send message from client\n");
			else
			printf("\nSERVER: Echoed back %s to %s.\n", buff, inet_ntoa(cli_addr.sin_addr));
		}
	}
}
