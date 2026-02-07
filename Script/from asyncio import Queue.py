from asyncio import Queue


class tree : 
    def __init__(self, value = 0, left = None, right = None) : 
        self.value = value
        self.left = left
        self.right = right


def DFS(B) : 
    if(B == None) : 
        return
    #prefix
    DFS(B.left)
    #infixe
    DFS(B.right)
    #sufixe

def check_sum(B):
    if B==None:
        return True
    if B.right==None and B.left!=None:
        return check_sum(B.left)
    if B.right!=None and B.left==None:
        return check_sum(B.right)
    if B.right==None and B.left==None:
        return True
    br=B.right.value
    bl=B.left.value
    if B.value!=br+bl:
        return False
    return check_sum(B.right) and check_sum(B.left)

def Increasing(B):
    if not B:
        return True
    Q=Queue(B)
    cpt=1
    while Q:
        cpt2=0
        for i in range(cpt):
            a=Q.Dequeue()
            if a.right:
                cpt2+=1
                Q.Enqueue(a.right)
            if a.left:
                cpt2+=1
                Q.Enqueue(a.left)
        if cpt>=cpt2:
            return False
        cpt=cpt2
    return True

B=None
B1=tree(75,None,tree(5,None,None))
B2=tree(1,tree(2,None,None),tree(3,None,None))

print(Increasing(B))
print(Increasing(B1))
print(Increasing(B2))