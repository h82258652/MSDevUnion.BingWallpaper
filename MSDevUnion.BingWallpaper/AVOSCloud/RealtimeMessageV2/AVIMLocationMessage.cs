using AVOSCloud;
using AVOSCloud.RealtimeMessage;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessageV2
{
    public class AVIMLocationMessage : AVIMTypedMessage
    {
        private IDictionary<string, object> _locationBody;

        public AVGeoPoint GeoPoint { get; set; }

        public override IDictionary<string, object> TypedMessageBody
        {
            get
            {
                this.typedMessageBody = base.TypedMessageBody;
                AVRMProtocolUtils.Write(this.typedMessageBody, AVIMProtocol.LCTYPE, (object)-5);
                if (this._locationBody == null)
                {
                    this._locationBody = (IDictionary<string, object>)new Dictionary<string, object>();
                    AVRMProtocolUtils.Write(this._locationBody, "latitude", (object)this.GeoPoint.Latitude);
                    AVRMProtocolUtils.Write(this._locationBody, "longitude", (object)this.GeoPoint.Longitude);
                }
                AVRMProtocolUtils.Write(this.typedMessageBody, AVIMProtocol.LCLOC, (object)this._locationBody);
                return this.typedMessageBody;
            }
            set
            {
                this.typedMessageBody = value;
            }
        }

        public AVIMLocationMessage()
        {
            this.MediaType = AVIMMessageMediaType.Location;
        }

        public AVIMLocationMessage(AVGeoPoint avGeoPoint)
        {
            this.GeoPoint = avGeoPoint;
        }

        public AVIMLocationMessage(double latitude, double longitude)
          : this(new AVGeoPoint()
          {
              Latitude = latitude,
              Longitude = longitude
          })
        {
        }

        public override void Deserialize(IDictionary<string, object> typedMessageBodyFromServer)
        {
            base.Deserialize(typedMessageBodyFromServer);
            this._locationBody = typedMessageBodyFromServer[AVIMProtocol.LCLOC] as IDictionary<string, object>;
            this.GeoPoint = new AVGeoPoint(Convert.ToDouble(this._locationBody["latitude"].ToString()), Convert.ToDouble(this._locationBody["longitude"].ToString()));
        }
    }
}