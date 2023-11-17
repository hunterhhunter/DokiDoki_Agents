class UN_Data():
    '''
    json으로 정보 교환.
    '''
    def __init__(self, persona, loc, obj) -> None:
        self.persona = persona
        self.location = loc
        self.object = obj

    def get_location(self):
        return self.location

    def get_persona(self):
        return self.persona

    def get_object(self):
        return self.object

    
