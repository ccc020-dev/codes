#include<stdio.h>
#include<stdlib.h>

struct Node {
    int data;
    struct Node *next;
};

struct Node *head = NULL;

// Traversal
void linkedListTraversal(struct Node *head){
    if(head == NULL){
        printf("List is empty!\n");
        return;
    }
    struct Node *ptr = head;
    do{
        printf("Element: %d\n", ptr->data);
        ptr = ptr->next;
    }while(ptr != head);
}

// Insert at beginning
struct Node * insertAtFirst(struct Node *head, int data){
    struct Node *ptr = (struct Node *) malloc(sizeof(struct Node));
    ptr->data = data;

    if(head == NULL){
        ptr->next = ptr;
        head = ptr;
        return head;
    }

    struct Node *p = head;
    while(p->next != head){
        p = p->next;
    }
    p->next = ptr;
    ptr->next = head;
    head = ptr;
    return head;
}

// Insert at end
struct Node * insertAtEnd(struct Node *head, int data){
    struct Node *ptr = (struct Node *) malloc(sizeof(struct Node));
    ptr->data = data;

    if(head == NULL){
        ptr->next = ptr;
        head = ptr;
        return head;
    }

    struct Node *p = head;
    while(p->next != head){
        p = p->next;
    }
    p->next = ptr;
    ptr->next = head;
    return head;
}

// Insert at any position
struct Node * insertAtPos(struct Node *head, int data, int pos){
    struct Node *ptr = (struct Node *) malloc(sizeof(struct Node));
    ptr->data = data;

    if(pos == 1 || head == NULL){   // insert at beginning
        return insertAtFirst(head, data);
    }

    struct Node *p = head;
    for(int i=1; i<pos-1 && p->next!=head; i++){
        p = p->next;
    }
    ptr->next = p->next;
    p->next = ptr;
    return head;
}

// Delete from beginning
struct Node * deleteFirst(struct Node *head){
    if(head == NULL){
        printf("List is empty!\n");
        return NULL;
    }
    struct Node *p = head;
    struct Node *q = head;

    while(q->next != head){
        q = q->next;
    }
    if(p->next == head){ // only 1 node
        free(p);
        return NULL;
    }
    q->next = head->next;
    head = head->next;
    free(p);
    return head;
}

// Delete from end
struct Node * deleteEnd(struct Node *head){
    if(head == NULL){
        printf("List is empty!\n");
        return NULL;
    }
    struct Node *p = head;
    struct Node *q = head;

    while(p->next != head){
        q = p;
        p = p->next;
    }
    if(p == head){ // only 1 node
        free(p);
        return NULL;
    }
    q->next = head;
    free(p);
    return head;
}

// Delete from any position
struct Node * deleteAtPos(struct Node *head, int pos){
    if(head == NULL){
        printf("List is empty!\n");
        return NULL;
    }

    if(pos == 1){
        return deleteFirst(head);
    }

    struct Node *p = head;
    for(int i=1; i<pos-1 && p->next!=head; i++){
        p = p->next;
    }
    struct Node *q = p->next;
    if(q == head){
        printf("Invalid position!\n");
        return head;
    }
    p->next = q->next;
    free(q);
    return head;
}

// Search element
void search(struct Node *head, int key){
    if(head == NULL){
        printf("List is empty!\n");
        return;
    }
    struct Node *ptr = head;
    int pos = 1;
    do{
        if(ptr->data == key){
            printf("Element %d found at position %d\n", key, pos);
            return;
        }
        ptr = ptr->next;
        pos++;
    }while(ptr != head);
    printf("Element %d not found!\n", key);
}

// Reverse the list
struct Node * reverse(struct Node *head){
    if(head == NULL || head->next == head)
        return head;

    struct Node *prev = NULL, *curr = head, *nextNode;
    struct Node *last = head;

    // find last node
    while(last->next != head)
        last = last->next;

    struct Node *stop = head;
    do{
        nextNode = curr->next;
        curr->next = prev ? prev : head;
        prev = curr;
        curr = nextNode;
    }while(curr != stop);

    head->next = prev;
    head = prev;
    return head;
}

// Main menu
int main(){
    int choice, data, pos;
    while(1){
        printf("\n--- Circular Linked List Menu ---\n");
        printf("1. Insert at Beginning\n");
        printf("2. Insert at End\n");
        printf("3. Insert at Position\n");
        printf("4. Delete from Beginning\n");
        printf("5. Delete from End\n");
        printf("6. Delete from Position\n");
        printf("7. Display\n");
        printf("8. Search\n");
        printf("9. Reverse\n");
        printf("10. Exit\n");
        printf("Enter your choice: ");
        scanf("%d", &choice);

        switch(choice){
            case 1:
                printf("Enter data: ");
                scanf("%d", &data);
                head = insertAtFirst(head, data);
                break;
            case 2:
                printf("Enter data: ");
                scanf("%d", &data);
                head = insertAtEnd(head, data);
                break;
            case 3:
                printf("Enter data: ");
                scanf("%d", &data);
                printf("Enter position: ");
                scanf("%d", &pos);
                head = insertAtPos(head, data, pos);
                break;
            case 4:
                head = deleteFirst(head);
                break;
            case 5:
                head = deleteEnd(head);
                break;
            case 6:
                printf("Enter position: ");
                scanf("%d", &pos);
                head = deleteAtPos(head, pos);
                break;
            case 7:
                linkedListTraversal(head);
                break;
            case 8:
                printf("Enter element to search: ");
                scanf("%d", &data);
                search(head, data);
                break;
            case 9:
                head = reverse(head);
                printf("List reversed!\n");
                break;
            case 10:
                exit(0);
            default:
                printf("Invalid choice!\n");
        }
    }
    return 0;
}
