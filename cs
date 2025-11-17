//TCP ITERATIVE CHAT SERVER PROGRAM

#include<sys/socket.h>
#include<sys/types.h>
#include<netinet/in.h>
#include<arpa/inet.h>
#include<string.h>
#include<stdio.h>
#include<stdlib.h>
#include<unistd.h>

struct sockaddr_in serv_addr,cli_addr;
int listenfd,connfd,r,w,val,cli_addr_len;
unsigned short serv_port = 25020; /*port number to be used by the server*/
char serv_ip[]="192.168.24.15";/*server's IP-ADDRESS*/
char buff[128];/*buffer for sending and receiving messages*/
char server_msg[128];/*buffer for server messages*/

int main()
{
    /*initialising server socket address structure with zero values*/
    bzero(&serv_addr,sizeof(serv_addr));
    
    /*filling up the server socket address with appropriate values*/
    serv_addr.sin_family=AF_INET;/*address family*/
    serv_addr.sin_port=htons(serv_port);/*port number*/
    inet_aton(serv_ip, (&serv_addr.sin_addr));/*IP-address*/
    
    printf("\nTCP ITERATIVE CHAT SERVER.\n");

    /*creating socket*/
    if((listenfd =socket(AF_INET,SOCK_STREAM,0))<0)
    {
        printf("\nSERVER ERROR: Cannot create socket.\n");
        exit(1);
    }
    
    /*binding server socket address structure*/
    if((bind(listenfd,(struct sockaddr*)&serv_addr,sizeof(serv_addr))) < 0)
    {
        printf("\nSERVER ERROR: Cannot bind\n");
        close(listenfd);
        exit(1);
    }
    
    /*listen to client connection request*/
    if((listen(listenfd, 5 ))< 0)
    {
        printf("\nSERVER ERROR: Cannot listen\n");
        close(listenfd);
        exit(1);
    }
    
    cli_addr_len=sizeof(cli_addr);
    
    for( ; ; )
    {
        printf("\nSERVER: Listening for clients...Press Ctrl+C to stop chat server\n");
        
        /*accept client connection*/
        if((connfd = accept(listenfd, (struct sockaddr*)&cli_addr,&cli_addr_len))<0)
        {
            printf("\nSERVER ERROR: Cannot accept client connections\n");
            close(listenfd);
            exit(1);
        }
        
        printf("\nSERVER: Connection from client %s accepted.\n",inet_ntoa(cli_addr.sin_addr));
        printf("SERVER: Chat session started. Type 'stop' to end session with this client.\n");
        
        /*Chat loop for current client*/
        while(1)
        {
            /*Clear buffers*/
            bzero(buff, sizeof(buff));
            bzero(server_msg, sizeof(server_msg));
            
            /*Receive message from client*/
            if((r = read(connfd, buff, sizeof(buff)-1)) <= 0)
            {
                printf("\nSERVER: Client disconnected or error in receiving message\n");
                break;
            }
            
            buff[r] = '\0';
            
            /*Check if client wants to quit*/
            if(strcmp(buff, "stop") == 0)
            {
                printf("\nSERVER: Client %s ended the chat session.\n", inet_ntoa(cli_addr.sin_addr));
                break;
            }
            
            printf("\nClient %s: %s\n", inet_ntoa(cli_addr.sin_addr), buff);
            
            /*Get server's response*/
            printf("Server: ");
            fgets(server_msg, sizeof(server_msg), stdin);

            /*Check if server wants to quit*/
            if(strcmp(server_msg, "stop\n") == 0)
            {
                strcpy(server_msg, "stop");
                write(connfd, server_msg, strlen(server_msg));
                printf("\nSERVER: Chat session ended by server.\n");
                break;
            }
            
            /*Send server's message to client*/
            if((w = write(connfd, server_msg, strlen(server_msg))) < 0)
            {
                printf("\nSERVER ERROR: Cannot send message to client\n");
                break;
            }
        }
        
        /*Close connection with current client*/
        close(connfd);
        printf("\nSERVER: Connection with client %s closed.\n", inet_ntoa(cli_addr.sin_addr));
    }/*outer for loop ends*/
    
    close(listenfd);
    return 0;
}/*main ends*/
