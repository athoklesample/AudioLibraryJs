﻿; (function ($, window, document, undefined) {
    $.widget("Mashup.audioSample", {

        //Options to be used as defaults
        options: {
            loop: false,
            fileName: null,
            sampleName: null,
            artist: null,
            bpm: 88,
            id: 0,
            rhythmIndex: 0,
            track: 0,
            mono: false,
            duration: 0,
            volume: 0.4096,
            pan: 0.5,
            effectType: -1,
            effect: 0.5,
            trimStart: 0,
            shiftSample: 0,
            playDuration: 0            
        },
        isGraphical: false,
        _trimEnd: 0,
        sampleUniqueId: 0,

        _create: function () {
            this._calculateStartBeat();
            this._calculateNumberOfBeats();
            this._loadSample();

            //create convolver
            this.convolver = this.isGraphical ? Mashup.Player.audioPlayerGraphical('getContext').createConvolver() :Mashup.Player.audioPlayer('getContext').createConvolver();
        },

        _calculateStartBeat: function () {
            var beatIncrement = this.isGraphical ? Mashup.Player.audioPlayerGraphical('option', 'beatIncrement') :Mashup.Player.audioPlayer('option', 'beatIncrement');
            this.startBeat = Math.round(this.options.rhythmIndex / beatIncrement);
        },

        _calculateNumberOfBeats: function () {
            this.numberOfBeats = Math.round((this.options.bpm / 60) * this.options.playDuration);
            //Some sound effect samples can be short and make numberOfBeats = 0.  At a minimum this should be 1 so it displays appropriately.
            if (this.numberOfBeats === 0)
                this.numberOfBeats = 1;
        },
        //typically this is passed in.  But if an edit is made to the number of beats, we need to calulate the new tempo
        _calculateTempo: function () {
            this.options.bpm = (this.numberOfBeats / this.options.duration) * 60;
        },
        _loadSample: function () {
            //first, check if samples has been loaded before
            var loadedSample = this.isGraphical ? Mashup.Player.audioPlayerGraphical('getLoadedSampleById', this.options.id) : Mashup.Player.audioPlayer('getLoadedSampleById', this.options.id); 
            if(typeof loadedSample.buffer != 'undefined') {
                this.buffer = loadedSample.buffer;
                return;
            }
            if(typeof loadedSample.id != 'undefined') {
                return;
            }
            loadedSample = {
                id: this.options.id
            };
            this.isGraphical ? Mashup.Player.audioPlayerGraphical('addLoadedSample', loadedSample) : Mashup.Player.audioPlayer('addLoadedSample', loadedSample);
                        
            var self = this;
            // Load asynchronously
            var url = Mashup.properties.sampleFilePath + this.options.fileName;
            var request = new XMLHttpRequest();
            request.open("GET", url, true);
            request.responseType = "arraybuffer";
            request.onload = function () {
                self.buffer = self.isGraphical ? Mashup.Player.audioPlayerGraphical('getContext').createBuffer(request.response, self.options.mono) : Mashup.Player.audioPlayer('getContext').createBuffer(request.response, self.options.mono);

                self.buffer.gain = self.options.volume;
                loadedSample = {
                    id: self.options.id,
                    buffer: self.buffer
                };
                self.isGraphical ? Mashup.Player.audioPlayerGraphical('updateLoadedSample', loadedSample) : Mashup.Player.audioPlayer('updateLoadedSample', loadedSample); 
            };
            request.send();
        },

        // Respond to any changes the user makes to the
        // option method
        _setOption: function (key, value) {
            switch (key) {
                case "volume":       
                    //http://www.dr-lex.be/info-stuff/volumecontrols.html#table1 -- doing the recommended x^4 
                    value = Math.pow(value, 4);
                    this.options.volume = value;
                    if (this.buffer !== null)
                        this.buffer.gain = value;
                    break;
                default:
                    this.options[key] = value;
                    break;
            }

             this._super( "_setOption", key, value );
        },

        getBuffer: function () {
            return this.buffer;
        },
        getConvolver: function () {
            return this.convolver;
        },
        getNumberOfBeats: function () {
            return this.numberOfBeats;
        },
        getStartBeat: function () {
            return this.startBeat;  
        },
        getOptions: function () {
            return this.options;
        },
        getUniqueIdentifier: function () {
            return this.sampleUniqueId;
        },
        setBuffer: function (buffer) {
            this.buffer = buffer;
            this.buffer.gain = this.options.volume;
        },
        setBufferNode: function (bufferNode) {
            this.bufferNode = bufferNode;
        },
        setUniqueIdentifier: function (id) {
            this.sampleUniqueId = id;
        },

        stop: function (noteTime) {
            if (this.bufferNode != null)
                this.bufferNode.noteOff(noteTime);
        },

        _destroy: function () {
        }

    });

})(jQuery, window, document);






